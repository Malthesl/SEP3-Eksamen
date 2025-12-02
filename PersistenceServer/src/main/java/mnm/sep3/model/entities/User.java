package mnm.sep3.model.entities;

public class User {
    private String username;
    private String password;
    private int id;

    public User(int id, String username) {
        this.id = id;
        this.username = username;
    }


    public User(int id, String username, String password) {
        this.id = id;
        this.username = username;
        this.password = password;
    }

    public String getUsername() {
        return username;
    }

    public String getPassword() {
        return password;
    }

    public int getId() {
        return id;
    }
}
