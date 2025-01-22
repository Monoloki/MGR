public interface IState {
    void Enter(); // Wywo³ywane, gdy postaæ przechodzi do tego stanu
    void Execute(); // Wywo³ywane w ka¿dej klatce podczas tego stanu
    void Exit(); // Wywo³ywane, gdy postaæ opuszcza ten stan
}