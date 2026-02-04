using System;

namespace Common.Net
{
    [Serializable]
    public class InputCommand
    {
        public InputType Type { get; set; }

        public InputCommand(InputType type)
        {
            Type = type;
        }
    }
}
