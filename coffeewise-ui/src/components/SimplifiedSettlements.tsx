import React, { useState, useEffect } from "react";
import {
  Typography,
  Paper,
  Alert,
  List,
  ListItemButton,
  ListItemText,
  CircularProgress,
} from "@mui/material";
import type { SettlementDto } from "../types/dto";
import { fetchSettlements } from "../api/coffeewise";

export default function SimplifiedSettlements({
  onSelectPair,
  refreshKey,
}: {
  onSelectPair: (fromId: string, toId: string) => void;
  refreshKey: number;
}) {
  const [settlements, setSettlements] = useState<SettlementDto[] | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    fetchSettlements()
      .then(setSettlements)
      .catch(() => setSettlements([]))
      .finally(() => setLoading(false));
  }, [refreshKey]);

  if (loading) {
    return (
      <Paper sx={{ p: 2, textAlign: "center" }}>
        <CircularProgress />
      </Paper>
    );
  }

  if (!settlements || settlements.length === 0) {
    return (
      <Alert severity="success" sx={{ p: 2 }}>
        Everyone is settled up! 🎉
      </Alert>
    );
  }

  return (
    <Paper sx={{ p: 2 }}>
      <Typography sx={{ mb: 2, fontWeight: "medium" }}>
        Click a settlement below to pre-fill the custom form.
      </Typography>
      <List dense>
        {settlements.map((s, idx) => (
          <ListItemButton
            key={idx}
            onClick={() => onSelectPair(s.fromPersonId, s.toPersonId)}
          >
            <ListItemText
              primary={
                <>
                  <strong>{s.fromName}</strong> pays <strong>{s.toName}</strong>{" "}
                  ${s.amount.toFixed(2)}
                </>
              }
            />
          </ListItemButton>
        ))}
      </List>
    </Paper>
  );
}
