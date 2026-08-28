import { cilAccountLogout, cilBuilding, cilUser } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CAvatar, CDropdown, CDropdownDivider, CDropdownItem, CDropdownMenu, CDropdownToggle } from "@coreui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../../store";
import { logout } from "../../features/auth/authSlice";

const AppHeaderDropdown = ({ onCompanySelect }) => {
    const navigate = useNavigate();
    const { t } = useTranslation();
    const user = useAppSelector((state) => state.auth.user);
    const companyName = useAppSelector((state) => state.auth.companyName);
    const [userName, setUserName] = useState("");

    useEffect(() => {
        if (user) {
            setUserName(user.name || user.email || t("auth.welcome"));
        }
    }, [user, t]);

    const handleLogout = () => {
        navigate("/auth/login");
    };

    const handleProfile = () => {
        navigate("/profile");
    };

    const handleCompanySelect = () => {
        if (onCompanySelect) {
            onCompanySelect();
        }
    };

    return (
        <CDropdown variant="nav-item">
            <CDropdownToggle className="py-0 pe-0" caret={false}>
                <CAvatar color="primary" textColor="white" size="md">
                    {userName.charAt(0).toUpperCase()}
                </CAvatar>
            </CDropdownToggle>
            <CDropdownMenu className="pt-0">
                <div className="p-3">
                    <div className="fw-semibold">{userName}</div>
                    <small className="text-muted" style={{ cursor: "pointer", textDecoration: "underline" }} onClick={handleCompanySelect} title={t("auth.selectCompany")}>
                        {companyName || t("auth.selectCompany")}
                    </small>
                </div>
                <CDropdownDivider />
                <CDropdownItem onClick={handleProfile}>
                    <CIcon icon={cilUser} className="me-2" />
                    {t("menu.profile")}
                </CDropdownItem>
                <CDropdownItem onClick={handleCompanySelect}>
                    <CIcon icon={cilBuilding} className="me-2" />
                    {t("auth.selectCompany")}
                </CDropdownItem>
                <CDropdownItem onClick={handleLogout}>
                    <CIcon icon={cilAccountLogout} className="me-2" />
                    {t("auth.logout")}
                </CDropdownItem>
            </CDropdownMenu>
        </CDropdown>
    );
};

export default AppHeaderDropdown;
