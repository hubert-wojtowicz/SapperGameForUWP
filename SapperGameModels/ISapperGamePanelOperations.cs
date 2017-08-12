using System.Collections.Generic;

namespace Saper.Model
{
    public interface ISapperGamePanelOperations
    {
        ushort GetHorizontalSize { get; }

        ushort GetVerticalSize { get; }

        ushort GetBombDensityPercent { get; }

        ushort GetNumberOfAdjacentBombsIn(Coordinate field);

        void UncoverZerosAndAdjacentIn(Coordinate currentField, ref List<Coordinate> listOfUncovered);

        void UncoverOneIn(Coordinate field);

        bool LeftFieldsUncoveredWithoutBomb();

        bool IsFieldUncovered(Coordinate field);

        bool IsFieldInsidePanel(Coordinate field);

        bool IsBombInside(Coordinate field);
    };
}
