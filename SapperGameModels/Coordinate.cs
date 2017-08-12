namespace Saper.Model
{
    public class Coordinate
    {
        public int horizontal;
        public int vertical;

        public Coordinate()
        { }

        public Coordinate(int hor, int ver)
        {
            horizontal = hor;
            vertical = ver;
        }

        public Coordinate(Coordinate currentPosition, int horizontalShift, int verticalShift)
        {
            horizontal = currentPosition.horizontal + horizontalShift;
            vertical = currentPosition.vertical + verticalShift;
        }
    }
}
