import { cilChart, cilPencil, cilPeople, cilPlus, cilTrash, cilTruck, cilWarning } from '@coreui/icons';
import CIcon from '@coreui/icons-react';
import {
    CAlert,
    CBadge,
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CCol,
    CContainer,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CNav,
    CNavItem,
    CNavLink,
    CRow,
    CSpinner,
    CTabContent,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow,
    CTabPane,
    CWidgetStatsA
} from '@coreui/react';
import { CChartBar } from '@coreui/react-chartjs';
import { getStyle } from '@coreui/utils';
import { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useSearchParams } from 'react-router-dom';
import EmployeeModal from '../../../components/EmployeeModal';
import Pagination from '../../../components/Pagination';
import { BasicEmployeeClient } from '../../../services/basic-employee-client';
import EmployeePerformanceClient from '../../../services/employee-performance-client';

const employeeClient = new BasicEmployeeClient("http://localhost:5083");
const performanceClient = new EmployeePerformanceClient();

const EmployeeList = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();

    // Tab state
    const [activeTab, setActiveTab] = useState(0);
    const [analyticsDays, setAnalyticsDays] = useState(90);

    const [employees, setEmployees] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [pagination, setPagination] = useState({
        page: 1,
        perPage: 10,
        total: 0,
        pages: 0
    });
    const [modalVisible, setModalVisible] = useState(false);
    const [deleteModalVisible, setDeleteModalVisible] = useState(false);
    const [selectedEmployee, setSelectedEmployee] = useState(null);
    const [employeeToDelete, setEmployeeToDelete] = useState(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    // Performance state
    const [performanceLoading, setPerformanceLoading] = useState(false);
    const [performance, setPerformance] = useState(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const employeeId = searchParams.get('id');
        if (employeeId) {
            loadEmployeeForEdit(employeeId);
        }
        loadEmployees();
    }, [pagination.page, pagination.perPage]);

    useEffect(() => {
        if (activeTab === 1) {
            loadPerformance();
        }
    }, [activeTab, analyticsDays]);

    const loadPerformance = async () => {
        try {
            setPerformanceLoading(true);
            const data = await performanceClient.getPerformance(analyticsDays);
            setPerformance(data);
        } catch (err) {
            setError(t('employees.performanceLoadError'));
            console.error('Failed to load employee performance:', err);
        } finally {
            setPerformanceLoading(false);
        }
    };

    const formatCurrency = (value) => {
        if (!value && value !== 0) return '-';
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(value);
    };

    const loadEmployeeForEdit = async (employeeId) => {
        try {
            const employee = await employeeClient.getById(employeeId);
            setSelectedEmployee(employee);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load employee for edit:', err);
            setError(t('employees.loadError'));
        }
    };

    const loadEmployees = async () => {
        try {
            setLoading(true);
            setError(null);
            const { page, perPage } = paginationRef.current;
            const response = await employeeClient.getAll(page, perPage);
            const isPaginated = response && response.data && Array.isArray(response.data);
            const employeesList = isPaginated ? response.data : (Array.isArray(response) ? response : []);
            const totalItems = response?.total ?? employeesList.length;
            setEmployees(employeesList);
            setPagination(prev => ({
                ...prev,
                total: totalItems,
                pages: Math.ceil(totalItems / prev.perPage) || 1
            }));
        } catch (err) {
            setError(t('employees.loadError'));
        } finally {
            setLoading(false);
        }
    };

    const handleOpenAdd = () => {
        setSelectedEmployee(null);
        setModalVisible(true);
    };

    const handleOpenEdit = async (employee) => {
        try {
            const fullEmployee = await employeeClient.getById(employee.id);
            setSelectedEmployee(fullEmployee);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load employee details:', err);
            setError(t('employees.loadError'));
        }
    };

    const handleOpenDelete = (employee) => {
        setEmployeeToDelete(employee);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData) => {
        setSaving(true);
        try {
            const payload = {
                id: formData.id || crypto.randomUUID(),
                positionId: formData.positionId,
                name: formData.name,
                email: formData.email,
                document: formData.document || null,
                city: formData.city || null,
                complement: formData.complement || null,
                neighborhood: formData.neighborhood || null,
                number: formData.number || null,
                stateId: formData.stateId,
                street: formData.street || null,
                zipCode: formData.zipCode || null,
                phoneNumber: formData.phoneNumber || null
            };

            if (selectedEmployee) {
                await employeeClient.update(selectedEmployee.id, payload);
                setSuccessMessage(t('employees.updateSuccess'));
            } else {
                await employeeClient.create(payload);
                setSuccessMessage(t('employees.createSuccess'));
            }
            setModalVisible(false);
            loadEmployees();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t('employees.saveError'));
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async () => {
        if (!employeeToDelete) return;

        setDeleting(true);
        try {
            await employeeClient.delete(employeeToDelete.id);
            setSuccessMessage(t('employees.deleteSuccess'));
            setDeleteModalVisible(false);
            setEmployeeToDelete(null);
            loadEmployees();
            setTimeout(() => setSuccessMessage(null), 5000);
        } catch (err) {
            setError(t('employees.loadError'));
        } finally {
            setDeleting(false);
        }
    };

    const handlePageChange = (newPage) => {
        setPagination(prev => ({ ...prev, page: newPage }));
    };

    const handlePerPageChange = (newPerPage) => {
        setPagination(prev => ({ ...prev, perPage: newPerPage, page: 1 }));
    };

    const formatPhone = (phone: string) => {
        if (!phone) return '-';
        const cleaned = phone.replace(/\D/g, '');
        if (cleaned.length === 10) {
            return `(${cleaned.slice(0,2)}) ${cleaned.slice(2,6)}-${cleaned.slice(6)}`;
        }
        return phone;
    };

    const getPerformanceLevelColor = (level: string) => {
        switch (level?.toLowerCase()) {
            case 'excellent': return 'success';
            case 'very good': return 'info';
            case 'good': return 'warning';
            default: return 'secondary';
        }
    };

    const getSalesChartData = () => {
        if (!performance || performance.salesByEmployee.length === 0) return null;

        return {
            labels: performance.salesByEmployee.slice(0, 10).map(e => e.employeeName.split(' ')[0]),
            datasets: [{
                label: t('employees.totalSales'),
                backgroundColor: getStyle('--cui-primary'),
                data: performance.salesByEmployee.slice(0, 10).map(e => e.totalSales),
            }]
        };
    };

    // Render Performance Tab Content
    const renderPerformanceTab = () => {
        if (performanceLoading) {
            return (
                <div className="text-center py-5">
                    <CSpinner color="primary" />
                    <p className="mt-3">{t('common.loading')}</p>
                </div>
            );
        }

        if (!performance) {
            return (
                <div className="text-center py-5">
                    <p className="text-muted">{t('common.noData')}</p>
                </div>
            );
        }

        return (
            <>
                {/* Time Range Selector */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <div className="d-flex justify-content-end gap-2">
                            <CButton size="sm" color={analyticsDays === 30 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(30)}>
                                {t('employees.last30Days')}
                            </CButton>
                            <CButton size="sm" color={analyticsDays === 90 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(90)}>
                                {t('employees.last90Days')}
                            </CButton>
                            <CButton size="sm" color={analyticsDays === 180 ? 'primary' : 'outline-primary'} onClick={() => setAnalyticsDays(180)}>
                                {t('employees.last180Days')}
                            </CButton>
                        </div>
                    </CCol>
                </CRow>

                {/* Summary Cards */}
                <CRow className="mb-4" xs={{ gutter: 4 }}>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="primary"
                            value={
                                <>
                                    {performance.summary.activeEmployees}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        / {performance.summary.totalEmployees} {t('employees.employees')}
                                    </span>
                                </>
                            }
                            title={t('employees.activeEmployees')}
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="success"
                            value={
                                <>
                                    {formatCurrency(performance.summary.totalSales)}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('employees.totalSales')}
                                    </span>
                                </>
                            }
                            title={t('employees.totalSales')}
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="info"
                            value={
                                <>
                                    {performance.summary.totalOrders}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('employees.totalOrders')}
                                    </span>
                                </>
                            }
                            title={t('employees.totalOrders')}
                        />
                    </CCol>
                    <CCol sm={6} xl={3}>
                        <CWidgetStatsA
                            color="warning"
                            value={
                                <>
                                    {formatCurrency(performance.summary.averageSalesPerEmployee)}
                                    <span className="fs-6 fw-normal d-block mt-1">
                                        {t('employees.avgPerEmployee')}
                                    </span>
                                </>
                            }
                            title={t('employees.averageSalesPerEmployee')}
                        />
                    </CCol>
                </CRow>

                {/* Sales by Employee Chart */}
                <CRow className="mb-4">
                    <CCol md={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilChart} className="me-2" />
                                <strong>{t('employees.salesByEmployee')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.salesByEmployee.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CChartBar
                                        data={getSalesChartData()}
                                        options={{
                                            responsive: true,
                                            maintainAspectRatio: true,
                                            plugins: {
                                                legend: {
                                                    display: false,
                                                },
                                            },
                                            scales: {
                                                x: {
                                                    grid: {
                                                        display: false,
                                                    },
                                                },
                                                y: {
                                                    beginAtZero: true,
                                                },
                                            },
                                        }}
                                    />
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>

                {/* Top Performers */}
                <CRow className="mb-4">
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilTruck} className="me-2 text-warning" />
                                <strong>{t('employees.topPerformers')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.topPerformers.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>#</CTableHeaderCell>
                                                <CTableHeaderCell>{t('employees.employee')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('employees.position')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('employees.orders')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('employees.sales')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('employees.performanceLevel')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {performance.topPerformers.map((performer, index) => (
                                                <CTableRow key={performer.employeeId}>
                                                    <CTableDataCell>
                                                        {index <= 2 ? (
                                                            <CIcon icon={cilTruck} className={`text-${index === 0 ? 'warning' : index === 1 ? 'secondary' : 'danger'}`} size="lg" />
                                                        ) : (
                                                            index + 1
                                                        )}
                                                    </CTableDataCell>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/employees?id=${performer.employeeId}`} className="text-decoration-none">
                                                            <strong>{performer.employeeName}</strong>
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell>{performer.positionName}</CTableDataCell>
                                                    <CTableDataCell className="text-center">{performer.totalOrders}</CTableDataCell>
                                                    <CTableDataCell className="text-end">
                                                        <strong>{formatCurrency(performer.totalSales)}</strong>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color={getPerformanceLevelColor(performer.performanceLevel)}>
                                                            {t(`employees.${performer.performanceLevel.toLowerCase().replace(' ', '')}`)}
                                                        </CBadge>
                                                    </CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>

                {/* Orders by Employee */}
                <CRow>
                    <CCol xs={12}>
                        <CCard>
                            <CCardHeader className="d-flex align-items-center">
                                <CIcon icon={cilPeople} className="me-2" />
                                <strong>{t('employees.ordersByEmployee')}</strong>
                            </CCardHeader>
                            <CCardBody>
                                {performance.ordersByEmployee.length === 0 ? (
                                    <p className="text-muted text-center">{t('common.noData')}</p>
                                ) : (
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('employees.employee')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('employees.position')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('employees.orders')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('employees.totalValue')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('employees.firstOrder')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-center">{t('employees.lastOrder')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {performance.ordersByEmployee.map((employee) => (
                                                <CTableRow key={employee.employeeId}>
                                                    <CTableDataCell>
                                                        <Link to={`/basic/employees?id=${employee.employeeId}`} className="text-decoration-none">
                                                            {employee.employeeName}
                                                        </Link>
                                                    </CTableDataCell>
                                                    <CTableDataCell>{employee.positionName}</CTableDataCell>
                                                    <CTableDataCell className="text-center">
                                                        <CBadge color="info">{employee.orderCount}</CBadge>
                                                    </CTableDataCell>
                                                    <CTableDataCell className="text-end">{formatCurrency(employee.totalValue)}</CTableDataCell>
                                                    <CTableDataCell className="text-center">{new Date(employee.firstOrderDate).toLocaleDateString()}</CTableDataCell>
                                                    <CTableDataCell className="text-center">{new Date(employee.lastOrderDate).toLocaleDateString()}</CTableDataCell>
                                                </CTableRow>
                                            ))}
                                        </CTableBody>
                                    </CTable>
                                )}
                            </CCardBody>
                        </CCard>
                    </CCol>
                </CRow>
            </>
        );
    };

    return (
        <CContainer className="py-4">
            {error && (
                <CAlert color="danger" dismissible onClose={() => setError(null)}>
                    {error}
                </CAlert>
            )}

            {successMessage && (
                <CAlert color="success" dismissible onClose={() => setSuccessMessage(null)}>
                    {successMessage}
                </CAlert>
            )}

            <CCard>
                <CCardHeader className="d-flex justify-content-between align-items-center">
                    <strong>{t('employees.title')}</strong>
                    <CButton color="primary" size="sm" onClick={handleOpenAdd}>
                        <CIcon icon={cilPlus} className="me-2" />
                        {t('employees.new')}
                    </CButton>
                </CCardHeader>
                <CCardBody>
                    {/* Main Navigation Tabs */}
                    <CNav variant="tabs">
                        <CNavItem>
                            <CNavLink active={activeTab === 0} onClick={() => setActiveTab(0)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilPeople} className="me-2" />
                                {t('employees.employeesList')}
                            </CNavLink>
                        </CNavItem>
                        <CNavItem>
                            <CNavLink active={activeTab === 1} onClick={() => setActiveTab(1)} style={{ cursor: 'pointer' }}>
                                <CIcon icon={cilChart} className="me-2" />
                                {t('employees.performance')}
                            </CNavLink>
                        </CNavItem>
                    </CNav>

                    <CTabContent className="mt-3">
                        {/* Employees List Tab */}
                        <CTabPane visible={activeTab === 0}>
                            {loading && (
                                <div className="text-center py-4">
                                    <CSpinner color="primary" />
                                    <p className="mt-2">{t('common.loading')}</p>
                                </div>
                            )}

                            {!loading && employees.length === 0 && (
                                <div className="text-center py-4">
                                    <p className="text-muted">{t('common.noData')}</p>
                                </div>
                            )}

                            {!loading && employees.length > 0 && (
                                <>
                                    <CTable hover responsive>
                                        <CTableHead>
                                            <CTableRow>
                                                <CTableHeaderCell>{t('employees.name')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('employees.email')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('employees.phone')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('employees.position')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('employees.city')}</CTableHeaderCell>
                                                <CTableHeaderCell>{t('employees.state')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {employees.map((employee) => (
                                                <CTableRow key={employee.id}>
                                                    <CTableDataCell>{employee.name}</CTableDataCell>
                                                    <CTableDataCell>{employee.email}</CTableDataCell>
                                                    <CTableDataCell>{formatPhone(employee.phoneNumber)}</CTableDataCell>
                                                    <CTableDataCell>
                                                        {employee.positionId ? (
                                                            <Link to={`/basic/positions?id=${employee.positionId}`} className="text-decoration-none">
                                                                {employee.positionName || '-'}
                                                            </Link>
                                                        ) : (
                                                    '-'
                                                )}
                                            </CTableDataCell>
                                            <CTableDataCell>{employee.city || '-'}</CTableDataCell>
                                            <CTableDataCell>{employee.stateName || '-'}</CTableDataCell>
                                            <CTableDataCell className="text-end">
                                                <CButton 
                                                    color="info" 
                                                    size="sm" 
                                                    className="me-2"
                                                    onClick={() => handleOpenEdit(employee)}
                                                >
                                                    <CIcon icon={cilPencil} />
                                                </CButton>
                                                <CButton 
                                                    color="danger" 
                                                    size="sm"
                                                    onClick={() => handleOpenDelete(employee)}
                                                >
                                                    <CIcon icon={cilTrash} />
                                                </CButton>
                                            </CTableDataCell>
                                        </CTableRow>
                                    ))}
                                </CTableBody>
                            </CTable>

                            <Pagination
                                pagination={pagination}
                                onPageChange={handlePageChange}
                                onPerPageChange={handlePerPageChange}
                            />
                        </>
                    )}
                        </CTabPane>
                        <CTabPane visible={activeTab === 1}>
                            {renderPerformanceTab()}
                        </CTabPane>
                    </CTabContent>
                </CCardBody>
            </CCard>

            <EmployeeModal
                visible={modalVisible}
                onClose={() => setModalVisible(false)}
                onSave={handleSave}
                employee={selectedEmployee}
                loading={saving}
            />

            <CModal 
                visible={deleteModalVisible} 
                onClose={() => setDeleteModalVisible(false)}
            >
                <CModalHeader>
                    <CModalTitle>
                        <CIcon icon={cilWarning} className="me-2 text-warning" />
                        {t('common.confirmDelete')}
                    </CModalTitle>
                </CModalHeader>
                <CModalBody>
                    <p>
                        {t('employees.deleteConfirm', { name: employeeToDelete?.name })}
                    </p>
                    <p className="text-danger">
                        {t('employees.deleteWarning')}
                    </p>
                </CModalBody>
                <CModalFooter>
                    <CButton 
                        color="secondary" 
                        onClick={() => setDeleteModalVisible(false)}
                        disabled={deleting}
                    >
                        {t('common.cancel')}
                    </CButton>
                    <CButton 
                        color="danger" 
                        onClick={handleDelete}
                        disabled={deleting}
                    >
                        {deleting ? t('common.deleting') : t('common.delete')}
                    </CButton>
                </CModalFooter>
            </CModal>
        </CContainer>
    );
};

export default EmployeeList;
