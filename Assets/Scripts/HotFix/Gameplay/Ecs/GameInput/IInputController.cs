using Saro.Entities;

namespace Tetris
{
    public interface IInputController
    {
        void BindInput(EcsWorld world);

        void ProcessInput();

        void OnDestroy() { }
    }
}
