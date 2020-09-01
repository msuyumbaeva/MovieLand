using GraphQL.Language.AST;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieLand.Api.GraphQL.GraphQLTypes
{
    internal class ByteValue : ValueNode<byte>
    {
        public ByteValue(byte value) {
            Value = value;
        }

        protected override bool Equals(ValueNode<byte> node) {
            return Value == node.Value;
        }
    }

    internal class ByteValueConverter : IAstFromValueConverter
    {
        public IValue Convert(object value, IGraphType type) {
            return new ByteValue((byte)value);
        }

        public bool Matches(object value, IGraphType type) {
            return value is byte;
        }
    }

    public class ByteGraphType : ScalarGraphType
    {
        public ByteGraphType() {
            Name = "Byte";
        }

        public override object ParseLiteral(IValue value) {
            var byteValue = value as ByteValue;
            return byteValue?.Value;
        }

        public override object ParseValue(object value) {
            if (value == null)
                return null;

            try {
                var result = Convert.ToByte(value);
                return result;
            }
            catch(FormatException) {
                return null;
            }
        }

        public override object Serialize(object value) {
            return ParseValue(value);
        }
    }
}
