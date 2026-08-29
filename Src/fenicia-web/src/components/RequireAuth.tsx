import { Navigate } from "react-router-dom";
import { useAppSelector } from "../store.ts";

const RequireAuth = ({ children }: { children: React.ReactNode }) => {
    const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
    if (!isAuthenticated) {
        return <Navigate to="/auth/login" replace />;
    }
    return <>{children}</>;
};

export default RequireAuth;
