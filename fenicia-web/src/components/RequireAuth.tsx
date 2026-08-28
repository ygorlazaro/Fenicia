import { Navigate } from "react-router-dom";
import { ApiClient } from "../services/api-client";

const RequireAuth = ({ children }: { children: React.ReactNode }) => {
    const apiClient = new ApiClient();
    const token = apiClient.getToken();
    if (!token) {
        return <Navigate to="/auth/login" replace />;
    }
    return <>{children}</>;
};

export default RequireAuth;
