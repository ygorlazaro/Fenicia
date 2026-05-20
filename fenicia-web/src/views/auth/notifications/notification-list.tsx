import { cilBell, cilSearch } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CAlert, CBadge, CCard, CCardBody, CCardHeader, CContainer, CFormInput, CSpinner, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import Pagination from "../../../components/fenicia/pagination";
import NotificationClient from "../../../services/notification/notification-client";
import { Notification } from "../../../types/notification/notification";
import formatDate from "../../../utils/format-date";

const notificationClient = new NotificationClient();

const NotificationList = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();

    const [notifications, setNotifications] = useState<Notification[]>([]);
    const [filteredNotifications, setFilteredNotifications] = useState<Notification[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [searchTerm, setSearchTerm] = useState("");

    const [pagination, setPagination] = useState({
        page: 1,
        perPage: 10,
        total: 0,
        pages: 0
    });

    useEffect(() => {
        loadNotifications();
    }, [pagination.page, pagination.perPage]);

    useEffect(() => {
        const filtered = notifications.filter((n) => n.title.toLowerCase().includes(searchTerm.toLowerCase()) || n.description.toLowerCase().includes(searchTerm.toLowerCase()));
        setFilteredNotifications(filtered);
    }, [searchTerm, notifications]);

    const loadNotifications = async () => {
        try {
            setLoading(true);
            setError(null);
            const response = await notificationClient.getAll(pagination.page, pagination.perPage);
            setNotifications(response.data);
            setFilteredNotifications(response.data);
            setPagination((prev) => ({
                ...prev,
                total: response.total,
                pages: response.pages || 1
            }));
        } catch (err) {
            console.error("Failed to load notifications:", err);
            setError(t("notifications.loadError"));
        } finally {
            setLoading(false);
        }
    };

    const handleRowClick = async (notification: Notification) => {
        if (!notification.read) {
            await notificationClient.markAsRead(notification.id);
        }
        navigate(`/notifications/${notification.id}`);
    };

    const handlePageChange = (newPage: number) => {
        setPagination((prev) => ({ ...prev, page: newPage }));
    };

    const handlePerPageChange = (newPerPage: number) => {
        setPagination((prev) => ({
            ...prev,
            perPage: newPerPage,
            page: 1,
            pages: Math.ceil(prev.total / newPerPage) || 1
        }));
    };

    const displayData = filteredNotifications;

    return (
        <CContainer className="py-4">
            {error && (
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error}
                </CAlert>
            )}

            <CCard>
                <CCardHeader className="d-flex justify-content-between align-items-center flex-wrap gap-2">
                    <div className="d-flex align-items-center gap-2">
                        <CIcon icon={cilBell} size="lg" />
                        <strong>{t("notifications.title")}</strong>
                    </div>
                    <div className="d-flex align-items-center gap-2" style={{ maxWidth: "300px", width: "100%" }}>
                        <CIcon icon={cilSearch} className="text-muted" />
                        <CFormInput type="text" placeholder={t("notifications.searchPlaceholder")} value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} size="sm" />
                    </div>
                </CCardHeader>
                <CCardBody>
                    {loading && (
                        <div className="text-center py-4">
                            <CSpinner color="primary" />
                            <p className="mt-2">{t("common.loading")}</p>
                        </div>
                    )}

                    {!loading && filteredNotifications.length === 0 && (
                        <div className="text-center py-4">
                            <p className="text-muted">{t("common.noData")}</p>
                        </div>
                    )}

                    {!loading && filteredNotifications.length > 0 && (
                        <>
                            <CTable hover responsive>
                                <CTableHead>
                                    <CTableRow>
                                        <CTableHeaderCell style={{ width: "50px" }}></CTableHeaderCell>
                                        <CTableHeaderCell>{t("notifications.date")}</CTableHeaderCell>
                                        <CTableHeaderCell>{t("notifications.title")}</CTableHeaderCell>
                                        <CTableHeaderCell>{t("notifications.description")}</CTableHeaderCell>
                                        <CTableHeaderCell style={{ width: "100px" }}>{t("notifications.status")}</CTableHeaderCell>
                                    </CTableRow>
                                </CTableHead>
                                <CTableBody>
                                    {displayData.map((notification) => (
                                        <CTableRow key={notification.id} onClick={() => handleRowClick(notification)} style={{ cursor: "pointer" }}>
                                            <CTableDataCell>
                                                {notification.imageUrl ? (
                                                    <img
                                                        src={notification.imageUrl}
                                                        alt=""
                                                        className="rounded"
                                                        style={{
                                                            width: "40px",
                                                            height: "40px",
                                                            objectFit: "cover"
                                                        }}
                                                    />
                                                ) : (
                                                    <div className="rounded bg-secondary d-flex align-items-center justify-content-center" style={{ width: "40px", height: "40px" }}>
                                                        <CIcon icon={cilBell} size="sm" className="text-white" />
                                                    </div>
                                                )}
                                            </CTableDataCell>
                                            <CTableDataCell>{formatDate(notification.date)}</CTableDataCell>
                                            <CTableDataCell>
                                                <strong className={!notification.read ? "text-primary" : ""}>{notification.title}</strong>
                                            </CTableDataCell>
                                            <CTableDataCell>
                                                <span className="text-truncate d-inline-block" style={{ maxWidth: "300px" }}>
                                                    {notification.description}
                                                </span>
                                            </CTableDataCell>
                                            <CTableDataCell>{notification.read ? <CBadge color="success">{t("notifications.read")}</CBadge> : <CBadge color="warning">{t("notifications.unread")}</CBadge>}</CTableDataCell>
                                        </CTableRow>
                                    ))}
                                </CTableBody>
                            </CTable>

                            <Pagination pagination={pagination} onPageChange={handlePageChange} onPerPageChange={handlePerPageChange} />
                        </>
                    )}
                </CCardBody>
            </CCard>
        </CContainer>
    );
};

export default NotificationList;
