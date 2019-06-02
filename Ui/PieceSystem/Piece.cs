using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace epona
{
    namespace ui
    {
        public abstract class PieceBase : MonoBehaviour
        {
            //public abstract bool Accepts(Type i_type);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////


        public interface IPieceBase
        {
            void ResetPiece();
        }

        public interface IPiece<T>: IPieceBase
        {
            void Configure(T i_data);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public abstract class Piece<T> : PieceBase, IPiece<T> where T : class
        {
            bool m_started = false;
            T m_data = null;
            private void Start()
            {
                m_started = true;

                StartImpl();

                if (m_started && m_data != null)
                {
                    Craft(m_data);
                    m_data = null;
                }
            }

            protected virtual void StartImpl()
            {

            }

            public virtual void ResetPiece()
            {

            }

            public void Configure(T i_data)
            {
                if (m_started)
                {
                    Craft(i_data);
                }
                else
                {
                    m_data = i_data;
                }
            }

            public abstract void Craft(T i_data);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public abstract class Piece<T1, T2> : Piece<T1>, IPiece<T2> where T1 : class where T2 : class
        {
            T1 m_data1 = null;
            T2 m_data2 = null;

            protected sealed override void StartImpl()
            {
                
            }

            public void Configure(T2 i_data2)
            {
                m_data2 = i_data2;
                if (m_data1 != null)
                {
                    Craft(m_data1, m_data2);
                    m_data1 = null;
                    m_data2 = null;
                }
            }

            public override void Craft(T1 i_data1)
            {
                m_data1 = i_data1;
                if (m_data2 != null)
                {
                    Craft(m_data1, m_data2);
                    m_data1 = null;
                    m_data2 = null;
                }
            }

            public override void ResetPiece()
            {
                base.ResetPiece();
            }

            public abstract void Craft(T1 i_data1, T2 i_data2);
            protected virtual void StartImpl2()
            {

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public abstract class PieceEx<T, UiElement> : Piece<T> where T : class where UiElement : MonoBehaviour
        {
            protected abstract void Craft(T i_data, UiElement i_interfaceElement);

            public sealed override void Craft(T i_data)
            {
                Craft(i_data, GetComponent<UiElement>());
            }

        }

        public abstract class PieceEx<T1, T2, UiElement> : Piece<T1, T2> where T1 : class where T2 : class where UiElement : MonoBehaviour
        {
            protected abstract void Craft(T1 i_data1, T2 i_data2, UiElement i_interfaceElement);

            public sealed override void Craft(T1 i_data1, T2 i_data2)
            {
                Craft(i_data1, i_data2, GetComponent<UiElement>());
            }

        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public abstract class PieceLabel<T> : PieceEx<T, UnityEngine.UI.Text> where T : class{ }
        public abstract class PieceText<T> : PieceEx<T, UnityEngine.UI.Text> where T : class { }
        public abstract class PieceLocalizedText<T> : PieceEx<T, LocalizedText> where T : class { }
        public abstract class PieceImage<T> : PieceEx<T, UnityEngine.UI.Image> where T : class { }
        public abstract class PieceRawImage<T> : PieceEx<T, UnityEngine.UI.RawImage> where T : class { }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        public abstract class PieceLabel<T1, T2> : PieceEx<T1, T2, UnityEngine.UI.Text> where T1 : class where T2 : class { }
        public abstract class PieceText<T1, T2> : PieceEx<T1, T2, UnityEngine.UI.Text> where T1 : class where T2 : class { }
        public abstract class PieceLocalizedText<T1, T2> : PieceEx<T1, T2, LocalizedText> where T1 : class where T2 : class { }
        public abstract class PieceImage<T1, T2> : PieceEx<T1, T2, UnityEngine.UI.Image> where T1 : class where T2 : class { }
        public abstract class PieceRawImage<T1, T2> : PieceEx<T1, T2, UnityEngine.UI.RawImage> where T1 : class where T2 : class { }
    }
}