using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Saper.Model
{
    public class SapperGamePanelModel : ISapperGamePanelOperations
    {
        public readonly ushort horizontalSize;
        public readonly ushort verticalSize;

        private readonly ushort bombsDensityPercent;
      
        private bool[,] isBombPositionTbl;
        private bool[,] isFieldUncoveredTbl;
 

        public SapperGamePanelModel(ushort horizontalSize, ushort verticalSize, ushort bombsDensityPercent=20)
        {
            if (horizontalSize < 3 || verticalSize < 3) throw new ArgumentException($"Inside {this.GetType().FullName} constructor too small size of panel defied!");

            this.horizontalSize = horizontalSize;
            this.verticalSize = verticalSize;

            if (bombsDensityPercent > 100)
            {
                // throwing exception in constructor is not good idea
                bombsDensityPercent = 20;
                Debug.WriteLine("WARNING: bombDensityPercent passed to SapperGamePanelModel exceeded 100%, therefore 20% set to avid unexpected behaviour");
            }
            this.bombsDensityPercent = bombsDensityPercent;

            isBombPositionTbl = new bool[horizontalSize, verticalSize];
            isFieldUncoveredTbl = new bool[horizontalSize, verticalSize];

            RegenerateBombsDistribution();
        }


        // called only once in constructor (one time per object life)
        protected void RegenerateBombsDistribution()
        {
            int howManyBombsLeftToPlace = (horizontalSize * verticalSize * bombsDensityPercent) / 100;

            Random pseudoNumberGenerator = new Random(DateTime.Now.Millisecond);
            int vertCord, horCord;

            while (--howManyBombsLeftToPlace >= 0)
            {
                do
                {
                    vertCord = pseudoNumberGenerator.Next(0, verticalSize - 1);
                    horCord = pseudoNumberGenerator.Next(0, horizontalSize - 1);
                }
                while (isBombPositionTbl[vertCord, horCord]);

                isBombPositionTbl[vertCord, horCord] = true;
            }
        }


        #region implementation of ISapperGamePanelOperations

        public ushort GetNumberOfAdjacentBombsIn(Coordinate field)
        {
            ushort numberOfAdjacentBombs = 0;

            // check all adjacent fields
            if (IsFieldInsidePanelAndHasBomb(new Coordinate(field.horizontal - 1, field.vertical - 1))) numberOfAdjacentBombs++;
            if (IsFieldInsidePanelAndHasBomb(new Coordinate(field.horizontal, field.vertical - 1))) numberOfAdjacentBombs++;
            if (IsFieldInsidePanelAndHasBomb(new Coordinate(field.horizontal + 1, field.vertical - 1))) numberOfAdjacentBombs++;

            if (IsFieldInsidePanelAndHasBomb(new Coordinate(field.horizontal - 1, field.vertical))) numberOfAdjacentBombs++;
            if (IsFieldInsidePanelAndHasBomb(new Coordinate(field.horizontal + 1, field.vertical))) numberOfAdjacentBombs++;

            if (IsFieldInsidePanelAndHasBomb(new Coordinate(field.horizontal - 1, field.vertical + 1))) numberOfAdjacentBombs++;
            if (IsFieldInsidePanelAndHasBomb(new Coordinate(field.horizontal, field.vertical + 1))) numberOfAdjacentBombs++;
            if (IsFieldInsidePanelAndHasBomb(new Coordinate(field.horizontal + 1, field.vertical + 1))) numberOfAdjacentBombs++;

            return numberOfAdjacentBombs;
        }

        public void UncoverZerosAndAdjacentIn(Coordinate currentField, ref List<Coordinate> listOfUncovered)
        {
            if (IsFieldInsidePanel(currentField) &&
                !isBombPositionTbl[currentField.horizontal, currentField.vertical] &&
                !isFieldUncoveredTbl[currentField.horizontal, currentField.vertical])
            {

                if (GetNumberOfAdjacentBombsIn(currentField) > 0)
                {
                    isFieldUncoveredTbl[currentField.horizontal, currentField.vertical] = true;
                    listOfUncovered.Add(currentField);
                }
                else // GetNumberOfAdjacentBombs == 0
                {
                    isFieldUncoveredTbl[currentField.horizontal, currentField.vertical] = true;
                    listOfUncovered.Add(currentField);


                    UncoverZerosAndAdjacentIn(new Coordinate(currentField, -1, -1), ref listOfUncovered);
                    UncoverZerosAndAdjacentIn(new Coordinate(currentField, 0, -1), ref listOfUncovered);
                    UncoverZerosAndAdjacentIn(new Coordinate(currentField, 1, -1), ref listOfUncovered);

                    UncoverZerosAndAdjacentIn(new Coordinate(currentField, -1, 0), ref listOfUncovered);
                    UncoverZerosAndAdjacentIn(new Coordinate(currentField, -1, 0), ref listOfUncovered);

                    UncoverZerosAndAdjacentIn(new Coordinate(currentField, -1, 1), ref listOfUncovered);
                    UncoverZerosAndAdjacentIn(new Coordinate(currentField, 0, 1), ref listOfUncovered);
                    UncoverZerosAndAdjacentIn(new Coordinate(currentField, 1, 1), ref listOfUncovered);
                    ;
                }
            }
        }

        public void UncoverOneIn(Coordinate field)
        {
            isFieldUncoveredTbl[field.horizontal, field.vertical] = true;
        }

        public bool LeftFieldsUncoveredWithoutBomb()
        {
            for (int i = 0; i < horizontalSize; i++)
            {
                for (int j = 0; j < verticalSize; j++)
                {
                    // using law: !a and !b <=> !(a or b)
                    if (!(isBombPositionTbl[i, j] || isFieldUncoveredTbl[i, j])) return true;
                }
            }
            return false;
        }

        public bool IsFieldUncovered(Coordinate field)
        {
            return isFieldUncoveredTbl[field.horizontal, field.vertical];
        }

        public bool IsFieldInsidePanel(Coordinate field)
        {
            if (field.horizontal >= 0 && field.horizontal < horizontalSize &&
                field.vertical >= 0 && field.vertical < verticalSize)
                return true;
            else
                return false;
        }

        public bool IsBombInside(Coordinate field)
        {
            return isBombPositionTbl[field.horizontal, field.vertical];
        }

        #endregion


        //helper method
        private bool IsFieldInsidePanelAndHasBomb(Coordinate field)
        {
            return IsFieldInsidePanel(field) && isBombPositionTbl[field.horizontal, field.vertical];
        }     
    }
}
