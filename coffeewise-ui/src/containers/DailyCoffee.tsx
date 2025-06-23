import { useEffect, useState, useMemo } from "react";
import { Box } from "@mui/material";
import useGroupMembers from "../hooks/useGroupMembers";
import {
  fetchRecommendedPayer,
  fetchPresenceFromServer,
  markPresence,
} from "../api/coffeewise";
import type { PersonDto } from "../types/dto";
import OrderForm from "../components/OrderForm";
import RecommendedPayer from "../components/RecommendedPayer";
import PresenceForm from "../components/PresenceForm";

export default function DailyCoffee({
  triggerRefresh,
}: {
  triggerRefresh: () => void;
}) {
  const members = useGroupMembers();

  const [presentMap, setPresentMap] = useState<Record<string, boolean>>({});
  const [changedLocally, setChangedLocally] = useState<Record<string, number>>(
    {}
  );
  const [recommendedPayer, setRecommendedPayer] = useState<PersonDto | null>(
    null
  );
  const [loadingRecommended, setLoadingRecommended] = useState(false);

  const presentPersonIds = useMemo(
    () =>
      members
        .filter((m) => presentMap[m.personId])
        .map((m) => m.personId)
        .sort(),
    [members, presentMap]
  );

  useEffect(() => {
    let cancelled = false;
    const fetchPayer = async () => {
      setLoadingRecommended(true);
      try {
        const payer = await fetchRecommendedPayer();
        if (!cancelled) setRecommendedPayer(payer);
      } finally {
        if (!cancelled) setLoadingRecommended(false);
      }
    };
    fetchPayer();
    return () => {
      cancelled = true;
    };
  }, [presentPersonIds.join(",")]);

  useEffect(() => {
    let cancelled = false;
    const poll = async () => {
      if (cancelled) return;
      try {
        const serverMap = await fetchPresenceFromServer();
        setPresentMap((prev) => {
          const now = Date.now();
          const merged: Record<string, boolean> = {};
          for (const m of members) {
            const id = m.personId;
            if (changedLocally[id] && now - changedLocally[id] < 5000) {
              merged[id] = prev[id] ?? false;
            } else {
              merged[id] = serverMap[id] ?? false;
            }
          }
          return merged;
        });
      } catch {}
    };

    poll();
    const interval = setInterval(poll, 5000);
    return () => {
      cancelled = true;
      clearInterval(interval);
    };
  }, [members, changedLocally]);

  const handlePresenceChange = async (personId: string, isPresent: boolean) => {
    setPresentMap((pm) => ({ ...pm, [personId]: isPresent }));
    setChangedLocally((cl) => ({ ...cl, [personId]: Date.now() }));
    try {
      await markPresence(personId, isPresent);
    } catch (err) {
      console.error("Failed to update presence.");
    }
  };
  return (
    <Box>
      <PresenceForm
        members={members}
        presentMap={presentMap}
        onChange={handlePresenceChange}
      />
      <RecommendedPayer
        recommended={recommendedPayer}
        loading={loadingRecommended}
      />
      <OrderForm
        members={members}
        presentMap={presentMap}
        recommendedPayer={recommendedPayer}
        onOrderSubmitted={triggerRefresh}
      />
    </Box>
  );
}
