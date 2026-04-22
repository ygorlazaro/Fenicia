import { cilChart, cilPencil, cilPeople, cilPlus, cilTrash, cilWarning } from '@coreui/icons';
import CIcon from '@coreui/icons-react';
import {
    CAlert,
    CButton,
    CCard,
    CCardBody,
    CCardHeader,
    CContainer,
    CModal,
    CModalBody,
    CModalFooter,
    CModalHeader,
    CModalTitle,
    CNav,
    CNavItem,
    CNavLink,
    CSpinner,
    CTabContent,
    CTable,
    CTableBody,
    CTableDataCell,
    CTableHead,
    CTableHeaderCell,
    CTableRow,
    CTabPane
} from '@coreui/react';
import { useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useSearchParams } from 'react-router-dom';
import Pagination from '../../../components/Pagination';
import { BasicEmployeeClient } from '../../../services/basic/basic-employee-client';
import { GetAllEmployeeResponse, GetEmployeeByIdResponse, UpdateEmployeeCommand } from '../../../types/basic-types';
import EmployeeModal from './employee-modal';
import { RenderPerformanceTab } from './performance';

const employeeClient = new BasicEmployeeClient("http://localhost:5083");

const EmployeeList = () => {
    const { t } = useTranslation();
    const [searchParams] = useSearchParams();

    // Tab state
    const [activeTab, setActiveTab] = useState(0);
    const [analyticsDays, setAnalyticsDays] = useState(90);

    const [employees, setEmployees] = useState<GetAllEmployeeResponse[]>([]);
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
    const [selectedEmployee, setSelectedEmployee] = useState<GetEmployeeByIdResponse | null>(null);
    const [employeeToDelete, setEmployeeToDelete] = useState<GetEmployeeByIdResponse | null>(null);
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);

    const paginationRef = useRef(pagination);
    paginationRef.current = pagination;

    useEffect(() => {
        const employeeId = searchParams.get('id');
        if (employeeId) {
            loadEmployeeForEdit(employeeId);
        }
        loadEmployees();
    }, [pagination.page, pagination.perPage]);

    const loadEmployeeForEdit = async (employeeId: string) => {
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

    const handleOpenEdit = async (employee: GetEmployeeByIdResponse) => {
        try {
            const fullEmployee = await employeeClient.getById(employee.id);
            setSelectedEmployee(fullEmployee);
            setModalVisible(true);
        } catch (err) {
            console.error('Failed to load employee details:', err);
            setError(t('employees.loadError'));
        }
    };

    const handleOpenDelete = (employee: GetEmployeeByIdResponse) => {
        setEmployeeToDelete(employee);
        setDeleteModalVisible(true);
    };

    const handleSave = async (formData: UpdateEmployeeCommand) => {
        setSaving(true);
        try {
            const payload: UpdateEmployeeCommand = {
                id: formData.id || crypto.randomUUID(),
                positionId: formData.positionId,
                name: formData.name,
                email: formData.email,
                document: formData.document || null,
                address: formData.address,
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

    const handlePageChange = (newPage: number) => {
        setPagination(prev => ({ ...prev, page: newPage }));
    };

    const handlePerPageChange = (newPerPage: number) => {
        setPagination(prev => ({ ...prev, perPage: newPerPage, page: 1 }));
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
                                                <CTableHeaderCell>{t('employees.position')}</CTableHeaderCell>
                                                <CTableHeaderCell className="text-end">{t('common.actions')}</CTableHeaderCell>
                                            </CTableRow>
                                        </CTableHead>
                                        <CTableBody>
                                            {employees.map((employee) => (
                                                <CTableRow key={employee.id}>
                                                    <CTableDataCell>{employee.name}</CTableDataCell>
                                                    <CTableDataCell>{employee.email}</CTableDataCell>
                                                    <CTableDataCell>
                                                        {employee.positionId ? (
                                                            <Link to={`/basic/positions?id=${employee.positionId}`} className="text-decoration-none">
                                                                {employee.positionName || '-'}
                                                            </Link>
                                                        ) : (
                                                            '-'
                                                        )}
                                                    </CTableDataCell>
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
                            <RenderPerformanceTab
                                analyticsDays={analyticsDays}
                                setAnalyticsDays={setAnalyticsDays}
                                activeTab={activeTab}
                                onError={t => setError(t)}
                            />
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
