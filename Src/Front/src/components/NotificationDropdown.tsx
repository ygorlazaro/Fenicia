import { cilBell } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CDropdown, CDropdownItem, CDropdownMenu, CDropdownToggle } from "@coreui/react";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import NotificationClient from "../services/notification/notification-client";
import { Notification } from "../types/notification/notification";
import formatDate from "../utils/format-date";

const notificationClient = new NotificationClient();

const NotificationDropdown = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [notifications, setNotifications] = useState<Notification[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadNotifications();
    }, []);

    const loadNotifications = async () => {
        try {
            setLoading(true);
            const data = await notificationClient.getRecent(5);
            setNotifications(data);
        } catch (err) {
            console.error("Failed to load notifications:", err);
        } finally {
            setLoading(false);
        }
    };

    const handleNotificationClick = async (notification: Notification) => {
        if (!notification.read) {
            await notificationClient.markAsRead(notification.id);
        }
        navigate(`/notifications/${notification.id}`);
    };

    const handleViewAll = () => {
        navigate("/notifications");
    };

    const unreadCount = notifications.filter((n) => !n.read).length;

    return (
        <CDropdown variant="nav-item">
            <CDropdownToggle caret={false} className="position-relative">
                <CIcon icon={cilBell} size="lg" />
                {unreadCount > 0 && (
                    <CBadge color="danger" position="top-end" shape="rounded-pill">
                        {unreadCount}
                    </CBadge>
                )}
            </CDropdownToggle>
            <CDropdownMenu className="pt-0" style={{ minWidth: "320px" }}>
                <div className="px-3 py-2 border-bottom d-flex justify-content-between align-items-center">
                    <strong>{t("notifications.title")}</strong>
                    {unreadCount > 0 && (
                        <CBadge color="danger" shape="rounded-pill">
                            {unreadCount}
                        </CBadge>
                    )}
                </div>

                {loading && (
                    <div className="text-center py-3">
                        <span className="text-muted">{t("common.loading")}</span>
                    </div>
                )}

                {!loading && notifications.length === 0 && (
                    <div className="text-center py-3">
                        <span className="text-muted">{t("common.noData")}</span>
                    </div>
                )}

                {!loading &&
                    notifications.map((notification) => (
                        <CDropdownItem key={notification.id} onClick={() => handleNotificationClick(notification)} style={{ cursor: "pointer" }} className="py-2">
                            <div className="d-flex align-items-start gap-2">
                                {notification.imageUrl && <img src={notification.imageUrl} alt="" className="rounded" style={{ width: "40px", height: "40px", objectFit: "cover", flexShrink: 0 }} />}
                                <div className="flex-grow-1" style={{ minWidth: 0 }}>
                                    <div className="d-flex justify-content-between align-items-start">
                                        <strong className={`d-block text-truncate ${!notification.read ? "text-primary" : ""}`} style={{ maxWidth: "200px" }}>
                                            {notification.title}
                                        </strong>
                                        {!notification.read && <span className="d-inline-block rounded-circle bg-primary ms-2" style={{ width: "8px", height: "8px", flexShrink: 0 }} />}
                                    </div>
                                    <small className="text-muted d-block text-truncate" style={{ maxWidth: "240px" }}>
                                        {notification.description}
                                    </small>
                                    <small className="text-muted">{formatDate(notification.date)}</small>
                                </div>
                            </div>
                        </CDropdownItem>
                    ))}

                <div className="border-top px-3 py-2 text-center">
                    <span onClick={handleViewAll} style={{ cursor: "pointer", fontSize: "0.875rem" }} className="text-primary">
                        {t("notifications.viewAll")}
                    </span>
                </div>
            </CDropdownMenu>
        </CDropdown>
    );
};

export default NotificationDropdown;
