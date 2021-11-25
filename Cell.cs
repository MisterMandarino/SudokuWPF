
namespace Sudoku2._0
{
    class Cell
    {
        #region Private Variables
        private int value;
        private bool isClue;
        #endregion

        #region Constructors
        public Cell()
        {
            this.value = 0;
            this.isClue = false;
        }
        public Cell(int value)
        {
            this.value = value;
            this.isClue = false;
        }
        #endregion

        #region Methods
        public bool IsClue()
        {
            return this.isClue;
        }

        public void SetIsClue(bool clue)
        {
            this.isClue = clue;
        }

        public void SetValue(int value)
        {
            this.value = value;
        }

        public int GetValue()
        {
            return this.value;
        }
        #endregion
    }
}
