import { cilArrowLeft, cilBell } from '@coreui/icons';
import CIcon from '@coreui/icons-react';
import {
  CAlert,
  CBadge,
  CButton,
  CCard,
  CCardBody,
  CCardHeader,
  CContainer,
  CSpinner,
} from '@coreui/react';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import NotificationClient from '../../../services/notification/notification-client';
import { Notification } from '../../../types/notification/notification';
import formatDate from '../../../utils/format-date';

const notificationClient = new NotificationClient();

const NotificationDetail = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const [notification, setNotification] = useState<Notification | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadNotification();
  }, [id]);

  const loadNotification = async () => {
    if (!id) return;

    try {
      setLoading(true);
      setError(null);
      const data = await notificationClient.getById(id);
      if (data) {
        setNotification(data);
        if (!data.read) {
          await notificationClient.markAsRead(id);
        }
      } else {
        setError(t('notifications.notFound'));
      }
    } catch (err) {
      console.error('Failed to load notification:', err);
      setError(t('notifications.loadError'));
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <CContainer className="py-4">
        <div className="text-center py-5">
          <CSpinner color="primary" />
          <p className="mt-3">{t('common.loading')}</p>
        </div>
      </CContainer>
    );
  }

  if (error || !notification) {
    return (
      <CContainer className="py-4">
        <CAlert color="danger" dismissible onClose={() => setError(null)}>
          {error || t('common.noData')}
        </CAlert>
        <CButton color="primary" onClick={() => navigate('/notifications')}>
          <CIcon icon={cilArrowLeft} className="me-2" />
          {t('common.back')}
        </CButton>
      </CContainer>
    );
  }

  return (
    <CContainer className="py-4">
      {/* Header Actions */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <CButton color="primary" onClick={() => navigate('/notifications')}>
          <CIcon icon={cilArrowLeft} className="me-2" />
          {t('common.back')}
        </CButton>
      </div>

      {/* Notification Detail Card */}
      <CCard>
        <CCardHeader className="d-flex justify-content-between align-items-center">
          <div className="d-flex align-items-center gap-2">
            <CIcon icon={cilBell} size="lg" />
            <strong>{t('notifications.detailTitle')}</strong>
          </div>
          {notification.read ? (
            <CBadge color="success">{t('notifications.read')}</CBadge>
          ) : (
            <CBadge color="warning">{t('notifications.unread')}</CBadge>
          )}
        </CCardHeader>
        <CCardBody>
          <div className="d-flex flex-column gap-3">
            {/* Date */}
            <div>
              <small className="text-muted text-uppercase fw-semibold">
                {t('notifications.date')}
              </small>
              <p className="mb-0">{formatDate(notification.date)}</p>
            </div>

            {/* Title */}
            <div>
              <small className="text-muted text-uppercase fw-semibold">
                {t('notifications.title')}
              </small>
              <h5 className="mb-0">{notification.title}</h5>
            </div>

            {/* Image */}
            {notification.imageUrl && (
              <div>
                <img
                  src={notification.imageUrl}
                  alt={notification.title}
                  className="rounded"
                  style={{ maxWidth: '100%', maxHeight: '300px', objectFit: 'cover' }}
                />
              </div>
            )}

            {/* Description */}
            <div>
              <small className="text-muted text-uppercase fw-semibold">
                {t('notifications.description')}
              </small>
              <p className="mb-0">{notification.description}</p>
            </div>
          </div>
        </CCardBody>
      </CCard>
    </CContainer>
  );
};

export default NotificationDetail;
