package mnm.sep3.server;

import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import mnm.sep3.CreateUserRequest;
import mnm.sep3.CreateUserResponse;
import mnm.sep3.DeleteUserRequest;
import mnm.sep3.Empty;
import mnm.sep3.GetUserByIdRequest;
import mnm.sep3.GetUserByIdResponse;
import mnm.sep3.GetUserByUsernameRequest;
import mnm.sep3.GetUserByUsernameResponse;
import mnm.sep3.UpdatePasswordRequest;
import mnm.sep3.UpdateUsernameRequest;
import mnm.sep3.UserDTO;
import mnm.sep3.UserServiceGrpc;
import mnm.sep3.VerifyUserCredentialsRequest;
import mnm.sep3.VerifyUserCredentialsResponse;
import mnm.sep3.model.User;
import mnm.sep3.model.UsersManager;

import java.util.List;

public class UserServiceImpl extends UserServiceGrpc.UserServiceImplBase {
    private final UsersManager usersManager;

    public UserServiceImpl(UsersManager usersManager) {
        this.usersManager = usersManager;
    }

    @Override
    public void createUser(CreateUserRequest request, StreamObserver<CreateUserResponse> responseStreamObserver) {
        CreateUserResponse.Builder response = CreateUserResponse.newBuilder();

        String username = request.getUsername();
        String password = request.getPassword();

        User returnUser = usersManager.createUser(username, password);

        UserDTO dto = UserDTO.newBuilder()
                .setId(returnUser.getId())
                .setUsername(returnUser.getUsername())
                .setPassword(returnUser.getPassword()).build();

        response.setUser(dto);

        responseStreamObserver.onNext(response.build());
        responseStreamObserver.onCompleted();
    }

    @Override
    public void getUserByUsername(GetUserByUsernameRequest request, StreamObserver<GetUserByUsernameResponse> responseStreamObserver) {
        try {
            GetUserByUsernameResponse.Builder response = GetUserByUsernameResponse.newBuilder();

            String username = request.getUsername();

            User returnUser = usersManager.getUserByUsername(username);

            UserDTO dto = UserDTO.newBuilder()
                    .setId(returnUser.getId())
                    .setUsername(returnUser.getUsername())
                    .setPassword(returnUser.getPassword()).build();

            response.setUser(dto);

            responseStreamObserver.onNext(response.build());
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(Status.NOT_FOUND
                    .withDescription("En fejl skete")
                    .augmentDescription(e.getMessage())
                    .asRuntimeException());
        }
    }

    @Override
    public void getUserById(GetUserByIdRequest request, StreamObserver<GetUserByIdResponse> responseStreamObserver) {
        try {
            GetUserByIdResponse.Builder response = GetUserByIdResponse.newBuilder();

            int id = request.getId();

            User returnUser = usersManager.getUserById(id);

            UserDTO dto = UserDTO.newBuilder()
                    .setId(returnUser.getId())
                    .setUsername(returnUser.getUsername())
                    .setPassword(returnUser.getPassword()).build();

            response.setUser(dto);

            responseStreamObserver.onNext(response.build());
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(Status.NOT_FOUND
                    .withDescription("En fejl skete")
                    .augmentDescription(e.getMessage())
                    .asRuntimeException());
        }
    }

    @Override
    public void updateUsername(UpdateUsernameRequest request, StreamObserver<Empty> emptyStreamObserver) {
        try {
            Empty.Builder response = Empty.newBuilder();

            int id = request.getId();
            String newUsername = request.getNewUsername();

            usersManager.updateUsername(id, newUsername);

            emptyStreamObserver.onNext(response.build());
            emptyStreamObserver.onCompleted();
        } catch (Exception e) {
            emptyStreamObserver.onError(Status.NOT_FOUND
                    .withDescription("En fejl skete")
                    .augmentDescription(e.getMessage())
                    .asRuntimeException());
        }
    }

    @Override
    public void updatePassword(UpdatePasswordRequest request, StreamObserver<Empty> emptyStreamObserver) {
        try {
            Empty.Builder response = Empty.newBuilder();

            int id = request.getId();
            String newPassword = request.getNewPassword();

            usersManager.updatePassword(id, newPassword);

            emptyStreamObserver.onNext(response.build());
            emptyStreamObserver.onCompleted();
        } catch (Exception e) {
            emptyStreamObserver.onError(Status.NOT_FOUND
                    .withDescription("En fejl skete")
                    .augmentDescription(e.getMessage())
                    .asRuntimeException());
        }
    }

    @Override
    public void deleteUser(DeleteUserRequest request, StreamObserver<Empty> emptyStreamObserver) {
        try {
            Empty.Builder response = Empty.newBuilder();

            int id = request.getId();

            usersManager.deleteUser(id);

            emptyStreamObserver.onNext(response.build());
            emptyStreamObserver.onCompleted();
        } catch (Exception e) {
            emptyStreamObserver.onError(Status.NOT_FOUND
                    .withDescription("En fejl skete")
                    .augmentDescription(e.getMessage())
                    .asRuntimeException());
        }
    }

    @Override
    public void verifyUserCredentials(VerifyUserCredentialsRequest request, StreamObserver<VerifyUserCredentialsResponse> responseStreamObserver) {
        try {
            VerifyUserCredentialsResponse.Builder response = VerifyUserCredentialsResponse.newBuilder();

            String username = request.getUsername();
            String password = request.getPassword();

            User returnUser = usersManager.verifyUserCredentials(username, password);

            UserDTO dto = UserDTO.newBuilder()
                    .setUsername(returnUser.getUsername())
                    .setPassword(returnUser.getPassword())
                    .setId(returnUser.getId())
                    .build();

            response.setUser(dto);

            responseStreamObserver.onNext(response.build());
            responseStreamObserver.onCompleted();
        } catch (Exception e) {
            responseStreamObserver.onError(Status.NOT_FOUND
                    .withDescription("En fejl skete")
                    .augmentDescription(e.getMessage())
                    .asRuntimeException());
        }
    }
}
