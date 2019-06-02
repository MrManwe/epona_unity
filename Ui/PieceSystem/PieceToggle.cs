namespace epona
{
    namespace ui
    {

        public abstract class UiPieceToggle<T> : Piece<T> where T : class
        {
            bool m_wasActiveInitially = false;
            public override sealed void Craft(T i_data)
            {
                bool shouldBeActive = ShouldBeActive(i_data);
                if (!shouldBeActive)
                {
                    gameObject.SetActive(false);
                }
            }

            protected sealed override void StartImpl()
            {
                m_wasActiveInitially = gameObject.activeSelf;
            }

            public sealed override void ResetPiece()
            {
                gameObject.SetActive(m_wasActiveInitially);
            }

            protected abstract bool ShouldBeActive(T i_data);
        }
    }
}