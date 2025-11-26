package mnm.sep3.model;

public interface UsersManager {
    /**
     * Opretter ny bruger
     * @param username Brugernavn på brugeren
     * @param password Password på brugeren
     * @return Den oprettede bruger
     * @throws RuntimeException Hvis brugeren ikke kunne oprettes
     */
    User createUser(String username, String password);

    /**
     * Henter bruger ud fra brugernavn
     * @return bruger med matchende brugernavn
     * @throws RuntimeException Hvis brugeren ikke kunne findes
     */
    User getUserByUsername(String username);

    /**
     * Henter bruger ud fra id
     * @return bruger med matchende id
     * @throws RuntimeException Hvis brugeren ikke kunne findes
     */
    User getUserById(int id);

    /**
     * Opdatere brugernavnet på en bruger
     * @param id Id'et på brugeren
     * @param newUsername Nye brugernavn
     * @throws RuntimeException Hvis brugeren ikke findes
     */
    void updateUsername(int id, String newUsername);

    /**
     * Opdatere passwordet på en bruger
     * @param id Id'et på brugeren
     * @param password Nye password
     * @throws RuntimeException Hvis brugeren ikke findes
     */
    void updatePassword(int id, String password);

    /**
     * Sletter en bruger ud fra et Id
     * @throws RuntimeException Hvis brugeren ikke findes
     */
    void deleteUser(int id);

    /**
     * Tjekker om en bruger har matchende brugernavn og password (TODO: Evt få den til at throw en checked)
     * @return Den bruger som blev logget ind med
     * @throws RuntimeException Hvis brugeren ikke findes
     */
    User verifyUserCredentials(String username, String password);
}
