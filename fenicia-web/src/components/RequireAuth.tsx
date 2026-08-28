import { Navigate } from "react-router-dom";

const TOKEN_KEY = "auth_token";

const RequireAuth = ({ children }: { children: React.ReactNode }) => {
    const token = localStorage.getItem(TOKEN_KEY);
    if (!token) {
        return <Navigate to="/auth/login" replace />;
    }
    return <>{children}</>;
};

export default RequireAuth;
