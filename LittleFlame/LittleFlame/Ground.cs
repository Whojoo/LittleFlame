namespace LittleFlame
{
    public class Ground
    {
        /// <summary>
        /// Contains the 3 types of grass: GRASS, DRYGRASS, SAND and ASH
        /// </summary>
        public enum GroundTypes
        {
            GRASS,
            GRASSFLOWERS,
            DRYGRASS,
            DRYGRASSFLOWERS,
            SAND,
            ASH,
            ROCK,
            MUD,
        }

        private int grounds;
        public int Grounds
        {
            get { return grounds; }
            set { grounds = value; }
        }

        private int burnValue;
        public int BurnValue
        {
            get { return burnValue; }
            set { burnValue = value; }
        }

        private float burned;
        public float Burned
        {
            get { return burned; }
            set { burned = value; }
        }


        private int redColor;


        /// <summary>
        /// Create a new Grass object.
        /// </summary>
        /// <param name="redColor">Gives the R (RGB) of color from the texturemapKey</param>
        public Ground(int redColor)
        {
            this.redColor = redColor;
            setupGroundType();
            burnValue = 30;

        }

        /// <summary>
        /// Sets the ground type of this object
        /// </summary>
        private void setupGroundType()
        {
            switch (redColor)
            {
                case 74:
                    grounds = (int)GroundTypes.GRASS;
                    burned = 1;
                    break;
                case 164:
                    grounds = (int)GroundTypes.DRYGRASS;
                    burned = 0.5f;
                    break;
                case 233:
                    grounds = (int)GroundTypes.SAND;
                    break;
                case 255:
                    grounds = (int)GroundTypes.ASH;
                    break;
                case 195:
                    grounds = (int)GroundTypes.ROCK;
                    break;
                case 80:
                    grounds = (int)GroundTypes.DRYGRASSFLOWERS;
                    break;
                case 100:
                    grounds = (int)GroundTypes.GRASSFLOWERS;
                    break;
                case 185:
                    grounds = (int)GroundTypes.MUD;
                    break;
                default:
                    //Nothing
                    break;
            }
        }

    }
}
