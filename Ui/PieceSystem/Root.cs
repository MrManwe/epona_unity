using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace epona
{
    namespace ui
    {

        class RootImpl<T> where T : class
        {
            abstract class PieceCacheBase<T2>
            {
                public abstract void Craft(T2 i_craft);
                public abstract void Reset();
                public abstract void Release();
            }

            class PieceCache<T2, R2> : PieceCacheBase<T2> where R2 : class where T2 : R2
            {
                IPiece<R2> m_piece;

                public PieceCache(IPiece<R2> i_piece)
                {
                    m_piece = i_piece;
                }

                public override void Craft(T2 i_data)
                {
                    m_piece.Configure(i_data);
                }

                public override void Reset()
                {
                    m_piece.ResetPiece();
                }

                public override void Release()
                {

                }
            }

            List<PieceCacheBase<T>> m_pieceCaches;

            public void Craft(T i_data, GameObject i_root)
            {
                Debug.Assert(i_data != null, "Passed Data cannot be null");
                if (m_pieceCaches != null)
                {
                    foreach (PieceCacheBase<T> cache in m_pieceCaches)
                    {
                        cache.Release();
                    }
                }
                m_pieceCaches = FillPieces<T, T>(i_data, i_root);

                foreach (PieceCacheBase<T> cache in m_pieceCaches)
                {
                    cache.Reset();
                }

                foreach (PieceCacheBase<T> cache in m_pieceCaches)
                {
                    cache.Craft(i_data);
                }
            }

            public void UpdatePieces(T i_data)
            {
                foreach (PieceCacheBase<T> cache in m_pieceCaches)
                {
                    cache.Reset();
                }

                foreach (PieceCacheBase<T> cache in m_pieceCaches)
                {
                    cache.Craft(i_data);
                }
            }

            //Do not use. It is public ONLY because of reflection
            private List<PieceCacheBase<T2>> FillPieces<T2, P>(P i_data, GameObject i_root) where P : class where T2 : P
            {
                List<PieceCacheBase<T2>> result = new List<PieceCacheBase<T2>>();
                foreach (IPiece<P> piece in i_root.GetComponentsInChildren<IPiece<P>>(true))
                {
                    result.Add(new PieceCache<T2, P>(piece));
                }

                //Some reflection power here!!!!
                var type = typeof(RootImpl<T>);
                var parseMethod = type.GetMethod("FillPieces", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                object[] parameters = new object[2];
                parameters[0] = i_data;
                parameters[1] = i_root;

                if (typeof(P).BaseType != null)
                {
                    var methodToCall = parseMethod.MakeGenericMethod(typeof(T2), typeof(P).BaseType);
                    result.AddRange((List<PieceCacheBase<T2>>)methodToCall.Invoke(this, parameters));
                }

                foreach (System.Type interfaceType in typeof(P).GetInterfaces())
                {
                    var methodToCall = parseMethod.MakeGenericMethod(typeof(T2), interfaceType);
                    result.AddRange((List<PieceCacheBase<T2>>)methodToCall.Invoke(this, parameters));
                }

                return result;
            }
        }

        public abstract class Root<T> : MonoBehaviour where T : class
        {
            RootImpl<T> m_impl = new RootImpl<T>();
            public void Craft(T i_data)
            {
                m_impl.Craft(i_data, gameObject);
            }

            public void UpdatePieces(T i_data)
            {
                m_impl.UpdatePieces(i_data);
            }
        }

        public abstract class Root<T1, T2> : MonoBehaviour where T1 : class where T2: class
        {
            RootImpl<T1> m_impl1 = new RootImpl<T1>();
            RootImpl<T2> m_impl2 = new RootImpl<T2>();

            public void Craft(T1 i_data1, T2 i_data2)
            {
                m_impl1.Craft(i_data1, gameObject);
                m_impl2.Craft(i_data2, gameObject);
            }

            public void UpdatePieces(T1 i_data1, T2 i_data2)
            {
                m_impl1.UpdatePieces(i_data1);
                m_impl2.UpdatePieces(i_data2);
            }
        }
    }
}