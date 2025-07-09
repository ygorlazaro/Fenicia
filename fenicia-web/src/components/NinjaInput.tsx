interface NinjaProps {
    label: string
    value: string
    onChange: (value: string) => void
    required?: boolean
    type?: "text" | "email" | "password"
}

export const NinjaInput = ({ label, value, onChange, required = true, type = "text" }: NinjaProps) => {
    return (
        <div className="mb-10">
            <div className="relative">
                <input className="block py-2.5 px-0 w-full text-sm text-gray-900 bg-transparent border-0 border-b-2 border-gray-300 appearance-none focus:outline-none focus:ring-0 focus:border-blue-600 peer"
                    type={type}
                    required={required}
                    value={value}
                    onChange={e => onChange(e.target.value)}
                />
                <label className="peer-focus:font-medium absolute text-sm text-gray-500 duration-300 transform -translate-y-6 scale-75 top-3 -z-10 origin-[0] peer-focus:start-0 rtl:peer-focus:translate-x-1/4 rtl:peer-focus:left-auto peer-focus:text-blue-600 peer-placeholder-shown:scale-100 peer-placeholder-shown:translate-y-0 peer-focus:scale-75 peer-focus:-translate-y-6">
                    {label}
                </label>
            </div>
            <p className="mt-2 text-sm text-red-600">
                <span className="font-medium">
                    Required
                </span>
            </p>
        </div>
    )
}
