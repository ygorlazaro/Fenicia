import Image from "next/image";

interface NinjaProps {
    children: React.ReactNode
}

export const NinjaAnonymousForm = ({ children }: NinjaProps) => {
    return (<div className="flex flex-row gap-4 p-16 border border-orange-400 rounded-2xl shadow m-20 ">

        <div className="gap-4">
            <Image src="/logo.jpeg" alt="Logo" width={600} height={600} className="mx-auto rounded-full ring-orange-400 ring-3" />
        </div>

        <div className="w-1/2 p-16 flex flex-col justify-center content-center">

            {children}
        </div>

    </div>)
}
