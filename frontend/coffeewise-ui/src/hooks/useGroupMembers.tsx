import { useEffect, useState } from "react";
import type { PersonDto } from "../types/dto";
import { fetchGroupMembers } from "../api/coffeewise";

export default function useGroupMembers(): PersonDto[] {
  const [members, setMembers] = useState<PersonDto[]>([]);
  useEffect(() => {
    fetchGroupMembers().then(setMembers).catch(console.error);
  }, []);
  return members;
}
