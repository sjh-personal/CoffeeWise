import React, { useState, useEffect } from "react";
import {
  Box,
  Paper,
  Typography,
  MenuItem,
  Select,
  Button,
  Stack,
  Alert,
} from "@mui/material";

import useGroupMembers from "../hooks/useGroupMembers";
import { fetchPairwise, settlePairwise } from "../api/coffeewise";

export default function PairwiseSettleUp({
  refreshBalances,
  prefillFrom,
  prefillTo,
}: {
  refreshBalances: () => void;
  prefillFrom?: string;
  prefillTo?: string;
}) {
  const members = useGroupMembers();
  const [payerId, setPayerId] = useState(prefillFrom || "");
  const [payeeId, setPayeeId] = useState(prefillTo || "");
  const [loading, setLoading] = useState(false);
  const [pairwise, setPairwise] = useState<null | {
    fromPersonId: string;
    toPersonId: string;
    amount: number;
  }>(null);
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    setPayerId(prefillFrom || "");
    setPayeeId(prefillTo || "");
  }, [prefillFrom, prefillTo]);

  useEffect(() => {
    if (!payerId || !payeeId || payerId === payeeId) {
      setPairwise(null);
      return;
    }
    setLoading(true);
    fetchPairwise(payerId, payeeId)
      .then(setPairwise)
      .finally(() => setLoading(false));
  }, [payerId, payeeId]);

  const handleSettle = async () => {
    if (!pairwise || pairwise.amount === 0) return;
    setLoading(true);
    console.log("testing", pairwise.amount);
    try {
      await settlePairwise(
        pairwise.fromPersonId,
        pairwise.toPersonId,
        pairwise.amount
      );
      setMessage("Settled!");
      setPairwise(null);
      setPayerId("");
      setPayeeId("");
      refreshBalances();
    } catch {
      setMessage("Error settling up.");
    } finally {
      setLoading(false);
      setTimeout(() => setMessage(null), 2500);
    }
  };

  const payer = members.find((m) => m.personId === payerId);
  const payee = members.find((m) => m.personId === payeeId);

  return (
    <Paper sx={{ mt: 4, p: 3 }}>
      <Typography variant="h6" sx={{ mb: 2 }}>
        Settle Up Between Two People
      </Typography>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        spacing={2}
        alignItems="center"
      >
        <Select
          value={payerId}
          onChange={(e) => setPayerId(e.target.value)}
          displayEmpty
          sx={{ minWidth: 180 }}
        >
          <MenuItem value="">Select Payer</MenuItem>
          {members.map((m) => (
            <MenuItem key={m.personId} value={m.personId}>
              {m.name}
            </MenuItem>
          ))}
        </Select>
        <Typography variant="body1" sx={{ alignSelf: "center" }}>
          →
        </Typography>
        <Select
          value={payeeId}
          onChange={(e) => setPayeeId(e.target.value)}
          displayEmpty
          sx={{ minWidth: 180 }}
        >
          <MenuItem value="">Select Payee</MenuItem>
          {members.map((m) => (
            <MenuItem key={m.personId} value={m.personId}>
              {m.name}
            </MenuItem>
          ))}
        </Select>
      </Stack>
      <Box sx={{ mt: 2 }}>
        {payerId && payeeId && payerId === payeeId && (
          <Alert severity="error">Select two different people.</Alert>
        )}
        {payerId && payeeId && payerId !== payeeId && pairwise && (
          <>
            {pairwise.fromPersonId === payerId &&
            pairwise.toPersonId === payeeId ? (
              pairwise.amount > 0 ? (
                <Alert severity="info">
                  <strong>{payer?.name}</strong> owes{" "}
                  <strong>{payee?.name}</strong>{" "}
                  <strong>${pairwise.amount.toFixed(2)}</strong>
                </Alert>
              ) : (
                <Alert severity="success">These two are all settled up!</Alert>
              )
            ) : (
              <>
                {pairwise.fromPersonId === payeeId &&
                pairwise.toPersonId === payerId &&
                pairwise.amount > 0 ? (
                  <Alert severity="warning">
                    <strong>{payee?.name}</strong> actually owes{" "}
                    <strong>{payer?.name}</strong>{" "}
                    <strong>${pairwise.amount.toFixed(2)}</strong>.
                  </Alert>
                ) : (
                  <Alert severity="warning">
                    <strong>{payer?.name}</strong> does not owe{" "}
                    <strong>{payee?.name}</strong> anything.
                  </Alert>
                )}
              </>
            )}
          </>
        )}
        {message && <Alert severity="success">{message}</Alert>}
      </Box>
      <Button
        sx={{ mt: 2 }}
        variant="contained"
        onClick={handleSettle}
        disabled={
          !pairwise ||
          pairwise.amount === 0 ||
          !payerId ||
          !payeeId ||
          payerId === payeeId ||
          pairwise.fromPersonId !== payerId ||
          pairwise.toPersonId !== payeeId ||
          loading
        }
      >
        Mark as Settled
      </Button>
    </Paper>
  );
}
