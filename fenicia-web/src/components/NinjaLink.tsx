import Link from "next/link";

interface NinjaProps {
    label: string
  href: string
  className?: string;
}

export const NinjaLink = ({ label, href, className = "" }: NinjaProps) => {
    return (
      <Link href={href} className={className + " text-orange-700"}>
              {label}
      </Link>
    )
            
}
