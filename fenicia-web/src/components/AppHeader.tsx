import { cilBuilding, cilMenu } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CContainer, CHeader, CHeaderNav, CHeaderToggler, CNavItem, CNavLink } from "@coreui/react";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { useAppDispatch, useAppSelector } from "../store";
import { setSidebarShow } from "../features/ui/uiSlice";
import { NavLink } from "react-router-dom";

import { ApiClient } from "../services/api-client";
import AuthCompanyClient from "../services/auth/auth-company-client";
import CompanySelectModal from "../views/auth/company/company-select-modal";
import { AppHeaderDropdown } from "./header/index";
import { AppBreadcrumb } from "./index";
import LanguageSelector from "./LanguageSelector";
import NotificationDropdown from "./NotificationDropdown";

const companyClient = new AuthCompanyClient("http://localhost:5144");
const apiClient = new ApiClient();

const AppHeader = () => {
    const headerRef = useRef<HTMLDivElement>(null);
    const { t } = useTranslation();
    const dispatch = useAppDispatch();
    const sidebarShow = useAppSelector((state) => state.ui.sidebarShow);
    const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
    const companyName = useAppSelector((state) => state.auth.companyName);

    // Company selection modal state
    const [showCompanyModal, setShowCompanyModal] = useState(false);
    const [companies, setCompanies] = useState([]);
    const [loadingCompanies, setLoadingCompanies] = useState(false);
    const [companiesError, setCompaniesError] = useState(null);

    useEffect(() => {
        document.addEventListener("scroll", () => {
            headerRef.current?.classList.toggle("shadow-sm", document.documentElement.scrollTop > 0);
        });
    }, []);

    const handleOpenCompanySelect = async () => {
        setShowCompanyModal(true);
        setLoadingCompanies(true);
        setCompaniesError(null);

        try {
            const response = await companyClient.getCompaniesByUser(1, 50);
            const companiesList = Array.isArray(response) ? response : response.data || [];
            setCompanies(companiesList);

            if (companiesList.length === 0) {
                setCompaniesError(t("auth.noCompanies"));
            }
        } catch (err) {
            console.error("Failed to load companies:", err);
            setCompaniesError(err.response?.data?.title || err.message || t("common.error"));
        } finally {
            setLoadingCompanies(false);
        }
    };

    const handleSelectCompany = (company) => {
        apiClient.setCompanyId(company.id);

        dispatch(setSidebarShow(false));

        setShowCompanyModal(false);

        window.location.reload();
    };

    // Translate menu items
    const menuItems = [
        { path: "/dashboard", label: t("menu.dashboard") },
        { path: "/profile", label: t("menu.profile") }
    ];

    return (
        <CHeader position="sticky" className="mb-4 p-0" ref={headerRef}>
            <CContainer className="border-bottom px-4" fluid>
                <CHeaderToggler onClick={() => dispatch(setSidebarShow(!sidebarShow))} style={{ marginInlineStart: "-14px" }}>
                    <CIcon icon={cilMenu} size="lg" />
                </CHeaderToggler>
                <CHeaderNav className="d-none d-md-flex">
                    {menuItems.map((item, index) => (
                        <CNavItem key={index}>
                            <CNavLink to={item.path} as={NavLink}>
                                {item.label}
                            </CNavLink>
                        </CNavItem>
                    ))}
                    {isAuthenticated && companyName && (
                        <CNavItem>
                            <CNavLink style={{ cursor: "pointer" }} onClick={handleOpenCompanySelect} title={t("auth.selectCompany")}>
                                <CIcon icon={cilBuilding} className="me-1" />
                                {companyName}
                            </CNavLink>
                        </CNavItem>
                    )}
                </CHeaderNav>
                <CHeaderNav>
                    {isAuthenticated && (
                        <>
                            <NotificationDropdown />
                            <li className="nav-item py-1">
                                <div className="vr h-100 mx-2 text-body text-opacity-75"></div>
                            </li>
                        </>
                    )}
                    <LanguageSelector />
                    <li className="nav-item py-1">
                        <div className="vr h-100 mx-2 text-body text-opacity-75"></div>
                    </li>
                    <AppHeaderDropdown onCompanySelect={handleOpenCompanySelect} />
                </CHeaderNav>
            </CContainer>
            <CContainer className="px-4" fluid>
                <AppBreadcrumb />
            </CContainer>

            {/* Company Select Modal */}
            <CompanySelectModal visible={showCompanyModal} companies={companies} loading={loadingCompanies} error={companiesError} onSelect={handleSelectCompany} />
        </CHeader>
    );
};

export default AppHeader;
