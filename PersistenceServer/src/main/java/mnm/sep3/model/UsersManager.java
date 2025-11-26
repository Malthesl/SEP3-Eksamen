package mnm.sep3.model;

public interface UsersManager {
    User createUser(String username, String password);
    User getUserByUsername(String username);
    User getUserById(int id);
    void updateUsername(int id, String newUsername);
    void updatePassword(int id, String password);
    void deleteUser(int id);
    User verifyUserCredentials(String username, String password);
}
