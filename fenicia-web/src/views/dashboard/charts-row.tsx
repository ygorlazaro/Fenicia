import {
    CRow
} from '@coreui/react';
import { FinancialAccountsReceivable, RevenueVsCost } from '../../services/financial-dashboard-client';
import { AccountsReceivable } from './accounts-receivable';
import { RevenuwVsCost } from './revenuw-vs-cost';

interface ChartsRowProps {
    revenueVsCost?: RevenueVsCost[];
    accountsReceivable?: FinancialAccountsReceivable;
}

const ChartsRow = ({ revenueVsCost = [], accountsReceivable }: ChartsRowProps) => {

    return (
        <CRow className="mb-4" xs={{ gutter: 4 }}>
            <RevenuwVsCost revenueVsCost={revenueVsCost} />

            <AccountsReceivable  accountsReceivable={accountsReceivable}  />
        </CRow>
    )
}

export default ChartsRow;
