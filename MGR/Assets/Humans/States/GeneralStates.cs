public interface IState {
    void Enter(); // Wywo�ywane, gdy posta� przechodzi do tego stanu
    void Execute(); // Wywo�ywane w ka�dej klatce podczas tego stanu
    void Exit(); // Wywo�ywane, gdy posta� opuszcza ten stan
}