import { cilTruck } from "@coreui/icons";
import CIcon from "@coreui/icons-react";
import { CBadge, CCard, CCardBody, CCardHeader, CCol, CRow, CTable, CTableBody, CTableDataCell, CTableHead, CTableHeaderCell, CTableRow } from "@coreui/react";
import { t } from "i18next";
import { Link } from "react-router-dom";
import { TopPerformer } from "../../../../services/employee-performance-client";
import { formatCurrency } from "../../../../utils/format-currency";

interface TopPerformersProps {
    topPerformers : TopPerformer[];
}

export function TopPerformers({ topPerformers }: TopPerformersProps) {
    const getPerformanceLevelColor = (level: string) => {
        switch (level?.toLowerCase()) {
            case 'excellent': return 'success';
            case 'very good': return 'info';
            case 'good': return 'warning';
            default: return 'secondary';
        }
    };

    return <CRow className="mb-4">
        <CCol xs={12}>
            <CCard>
                <CCardHeader className="d-flex align-items-center">
                    <CIcon icon={cilTruck} className="me-2 text-warning" />
                    <strong>{t('employees.topPerformers')}</strong>
                </CCardHeader>
                <CCardBody>
                    {topPerformers.length === 0 ? <p className="text-muted text-center">{t('common.noData')}</p> : <CTable hover responsive>
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
                            {topPerformers.map((performer, index) => <CTableRow key={performer.employeeId}>
                                <CTableDataCell>
                                    {index <= 2 ? <CIcon icon={cilTruck} className={`text-${index === 0 ? 'warning' : index === 1 ? 'secondary' : 'danger'}`} size="lg" /> : index + 1}
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
                            </CTableRow>)}
                        </CTableBody>
                    </CTable>}
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>;
}
