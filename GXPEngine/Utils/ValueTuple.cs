namespace GXPEngine {
    /// <summary>
    /// This class exists only because System.ValueTuple wes added in .NET framework v4.7
    /// </summary>
    public class ValueTuple<TA,TB> {
        public TA Item1;
        public TB Item2;

        public static ValueTuple<TA, TB> Create(TA value1, TB value2) {
            ValueTuple<TA, TB> valueTuple = new ValueTuple<TA, TB>();
            valueTuple.Item1 = value1;
            valueTuple.Item2 = value2;
            return valueTuple;
        }
    }
}