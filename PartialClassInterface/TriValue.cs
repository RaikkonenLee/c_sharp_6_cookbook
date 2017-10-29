using System;

namespace PartialClassInterfaces
{
    public partial class TriValue
    {
        public decimal First { get; set; }
        public decimal Second { get; set; }
        public decimal Third { get; set; }

        public TriValue(decimal first, decimal second, decimal third)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
        }

        public TypeCode GetTypeCode() => TypeCode.Object;

        public decimal Average => (Sum / 3);

        public decimal Sum => First + Second + Third;

        public decimal Product => First * Second * Third;
    }
}
