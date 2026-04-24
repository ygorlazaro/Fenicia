import { CFormLabel, CFormSelect } from "@coreui/react";
import { useTranslation } from "react-i18next";

interface FeniciaSelectProps {
  onChange: (e: React.ChangeEvent<HTMLSelectElement>) => void;
  data: { id: string; name: string }[];
  loadingOptions?: boolean;
  value: string;
  label: string;
  id: string;
  required?: boolean;
}

export function FeniciaSelect({
  onChange,
  data,
  value,
  loadingOptions = false,
  label,
  id,
  required = false
}: FeniciaSelectProps) {
  const { t } = useTranslation();

  return <>
    <CFormLabel htmlFor={id}>{t(label)} {required && <span className="text-danger">*</span>}</CFormLabel>
    <CFormSelect id={id} name={id} value={value} onChange={onChange} disabled={loadingOptions} required>
      <option value="">{t('common.select')}...</option>
      {data.map(item => <option key={item.id} value={item.id}>
        {item.name}
      </option>)}
    </CFormSelect>
  </>;
}
