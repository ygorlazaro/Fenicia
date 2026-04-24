import {
    CRow
} from '@coreui/react';
import { FinancialAccountsReceivable } from '../../types/basic/dashboard/financial-accounts-receivable';
import { RevenueVsCost } from '../../types/basic/dashboard/revenue-vs-cost';
import AccountsReceivable from './accounts-receivable';
import RevenuwVsCost from './revenuw-vs-cost';

interface ChartsRowProps {
    revenueVsCost?: RevenueVsCost[];
    accountsReceivable?: FinancialAccountsReceivable;
}

const ChartsRow = ({ revenueVsCost = [], accountsReceivable }: ChartsRowProps) => {

    return (
        <CRow className="mb-4" xs={{ gutter: 4 }}>
            <RevenuwVsCost revenueVsCost={revenueVsCost} />

            <AccountsReceivable data={accountsReceivable} />
        </CRow>
    )
}

export default ChartsRow;
