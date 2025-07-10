'use client'

import { NinjaAnonymousForm } from "@/components/NinjaAnonymousForm";
import { NinjaButton } from "@/components/NinjaButton";
import { NinjaInput } from "@/components/NinjaInput";
import { NinjaLink } from "@/components/NinjaLink";
import { NinjaMessage } from "@/components/NinjaMessage";
import { setToken, setUser } from "@/logic/token";
import { TokenService } from "@/services/TokenService";
import { TokenRequest } from "@/types/requests/TokenRequest";
import { useRouter } from "next/navigation";
import { useState } from "react";

const tokenService = new TokenService();

export default function Home() {
  const [request, setRequest] = useState<TokenRequest>({
    email: "ygor@ygorlazaro.com",
    password: "Age14rjy",
    cnpj: "23351185000184"
  });
  const [message, setMessage] = useState<string | undefined>();
  const [loading, setLoading] = useState<boolean>(false);
  const router = useRouter();

  const handleSubmit = () => {
    const submit = (async () => {
      try {
        setLoading(true);
        const token = await tokenService.getToken(request);

        setToken(token.accessToken);
        setUser(token.user);
        router.push("/dashboard")
      } catch (error: any) {
        setMessage(error.response?.data.message);
      }
      finally {
        setLoading(false);
      }
    })

    submit();
  }


  return (
    <NinjaAnonymousForm title="Login">

      <NinjaInput required label="Email" value={request.email} type="email" onChange={(value => setRequest({ ...request, email: value }))} />
      <NinjaInput required label="Password" value={request.password} type="password" onChange={(value => setRequest({ ...request, password: value }))} />
      <NinjaInput required label="CNPJ" value={request.cnpj} onChange={(value => setRequest({ ...request, cnpj: value }))} />

      <NinjaButton label="Login" onClick={handleSubmit} loading={loading} />

      <NinjaLink label="Sign Up" href="/signup" className="self-end" />

      <NinjaMessage message={message} />

    </NinjaAnonymousForm>
  );
}
