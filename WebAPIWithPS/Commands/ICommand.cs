namespace WebAPIWithPS.Commands
{
    using System.Collections.Generic;

    public interface ICommand
    {
        void Init(Dictionary<string, object> data);
        string Execute();
        object Result { get; set; }

    }
}