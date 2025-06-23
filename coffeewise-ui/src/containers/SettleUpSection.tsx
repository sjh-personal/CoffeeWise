import React, { useState, useEffect } from "react";
import {
  Box,
  Typography,
  Paper,
  RadioGroup,
  FormControlLabel,
  Radio,
  Select,
  MenuItem,
  TextField,
  Button,
  List,
  ListItemButton,
  ListItemText,
  CircularProgress,
  Alert,
  Stack,
  Divider,
} from "@mui/material";
import ArrowRightAltIcon from "@mui/icons-material/ArrowRightAlt";
import useGroupMembers from "../hooks/useGroupMembers";
import {
  fetchPairwisePositions,
  fetchSettlements,
  settlePairwise,
} from "../api/coffeewise";
import type { PairwiseBalanceDto, SettlementDto } from "../types/dto";

export enum MethodType {
  Direct = "direct",
  Optimized = "optimized",
  Custom = "custom",
}

export default function SettleUpUnified({
  refreshBalances,
  refreshKey,
}: {
  refreshBalances: () => void;
  refreshKey: number;
}) {
  const members = useGroupMembers();
  const [method, setMethod] = useState<MethodType>(MethodType.Direct);
  const [payerId, setPayerId] = useState<string>("");
  const [payeeId, setPayeeId] = useState<string>("");
  const [amount, setAmount] = useState<number>(0);

  const [directList, setDirectList] = useState<PairwiseBalanceDto[] | null>(
    null
  );
  const [optList, setOptList] = useState<SettlementDto[] | null>(null);
  const [loadingList, setLoadingList] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [selectedKey, setSelectedKey] = useState<string | null>(null);

  useEffect(() => {
    setLoadingList(true);
    Promise.all([fetchPairwisePositions(), fetchSettlements()])
      .then(([pairs, opts]) => {
        setDirectList(pairs);
        setOptList(opts);
      })
      .catch(() => {
        setDirectList([]);
        setOptList([]);
      })
      .finally(() => setLoadingList(false));
  }, [refreshKey]);

  useEffect(() => {
    if (!payerId || !payeeId || payerId === payeeId) {
      setAmount(0);
      return;
    }

    if (method === MethodType.Direct && directList) {
      const pair = directList.find(
        (d) => d.fromPersonId === payerId && d.toPersonId === payeeId
      );
      setAmount(pair?.amount ?? 0);
    }

    if (method === MethodType.Optimized && optList) {
      const opt = optList.find(
        (s) => s.fromPersonId === payerId && s.toPersonId === payeeId
      );
      setAmount(opt?.amount ?? 0);
    }

    if (method === MethodType.Custom) {
      setAmount(0);
    }
  }, [method, payerId, payeeId, directList, optList]);

  const handleSubmit = async () => {
    if (!payerId || !payeeId || payerId === payeeId || amount <= 0) return;
    setSubmitting(true);
    try {
      await settlePairwise(payerId, payeeId, amount);
      setMessage("Settlement recorded!");
      setPayerId("");
      setPayeeId("");
      setAmount(0);
      setSelectedKey(null);
      refreshBalances();
    } catch {
      setMessage("Error saving settlement.");
    } finally {
      setSubmitting(false);
      setTimeout(() => setMessage(null), 3000);
    }
  };

  const onSelectSuggestion = (
    type: MethodType,
    from: string,
    to: string,
    amt: number,
    key: string
  ) => {
    setMethod(type);
    setPayerId(from);
    setPayeeId(to);
    setAmount(amt);
    setSelectedKey(key);
  };

  if (loadingList) {
    return (
      <Paper sx={{ p: 2, textAlign: "center" }}>
        <CircularProgress />
      </Paper>
    );
  }

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Settle Up
      </Typography>
      <Typography variant="body2" color="textSecondary" gutterBottom>
        Select a payment method and fill the details below.
      </Typography>

      <RadioGroup
        row
        value={method}
        onChange={(_, v) => setMethod(v as MethodType)}
        sx={{ mb: 1 }}
      >
        <FormControlLabel
          value={MethodType.Direct}
          control={<Radio />}
          label="Direct"
        />
        <FormControlLabel
          value={MethodType.Optimized}
          control={<Radio />}
          label="Optimized"
        />
        <FormControlLabel
          value={MethodType.Custom}
          control={<Radio />}
          label="Custom"
        />
      </RadioGroup>

      <Typography variant="body2" sx={{ mb: 2 }}>
        <strong>Direct:</strong> Pay someone you directly owe.
        <br />
        <strong>Optimized:</strong> Minimize total payments across the group by
        consolidating debts.
      </Typography>

      <Stack direction={{ xs: "column", sm: "row" }} spacing={2} mb={2}>
        <Select
          value={payerId}
          onChange={(e) => setPayerId(e.target.value)}
          displayEmpty
          sx={{ minWidth: 160 }}
        >
          <MenuItem value="">Payer</MenuItem>
          {members.map((m) => (
            <MenuItem key={m.personId} value={m.personId}>
              {m.name}
            </MenuItem>
          ))}
        </Select>

        <Select
          value={payeeId}
          onChange={(e) => setPayeeId(e.target.value)}
          displayEmpty
          sx={{ minWidth: 160 }}
        >
          <MenuItem value="">Payee</MenuItem>
          {members.map((m) => (
            <MenuItem key={m.personId} value={m.personId}>
              {m.name}
            </MenuItem>
          ))}
        </Select>

        <TextField
          type="number"
          value={amount}
          onChange={(e) => setAmount(+e.target.value)}
          label="Amount"
          disabled={method !== MethodType.Custom}
          sx={{ maxWidth: 120 }}
        />
      </Stack>

      {payerId && payeeId && payerId === payeeId && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Cannot settle with yourself.
        </Alert>
      )}

      <Button
        variant="contained"
        onClick={handleSubmit}
        disabled={
          !payerId ||
          !payeeId ||
          payerId === payeeId ||
          amount <= 0 ||
          submitting
        }
      >
        Submit Settlement
      </Button>

      {message && <Alert sx={{ mt: 2 }}>{message}</Alert>}

      <Divider sx={{ my: 4 }} />

      <Typography variant="subtitle1" gutterBottom>
        Quick Suggestions
      </Typography>

      <Typography variant="subtitle2" gutterBottom>
        Direct Debts
      </Typography>
      <List dense>
        {directList!.map((d) => {
          const key = `direct-${d.fromPersonId}-${d.toPersonId}`;
          return (
            <ListItemButton
              key={key}
              selected={selectedKey === key}
              onClick={() =>
                onSelectSuggestion(
                  MethodType.Direct,
                  d.fromPersonId,
                  d.toPersonId,
                  d.amount,
                  key
                )
              }
            >
              <ListItemText
                primary={
                  <Box display="flex" alignItems="center">
                    <strong>{d.fromPersonName}</strong>
                    <ArrowRightAltIcon
                      sx={{ mx: 1, verticalAlign: "middle" }}
                    />
                    <strong>{d.toPersonName}</strong>
                    <Box component="span" ml={1}>
                      ${d.amount.toFixed(2)}
                    </Box>
                  </Box>
                }
              />
            </ListItemButton>
          );
        })}
      </List>

      <Typography variant="subtitle2" gutterBottom sx={{ mt: 2 }}>
        Group-Optimized
      </Typography>
      <List dense>
        {optList!.map((s) => {
          const key = `opt-${s.fromPersonId}-${s.toPersonId}`;
          return (
            <ListItemButton
              key={key}
              selected={selectedKey === key}
              onClick={() =>
                onSelectSuggestion(
                  MethodType.Optimized,
                  s.fromPersonId,
                  s.toPersonId,
                  s.amount,
                  key
                )
              }
            >
              <ListItemText
                primary={
                  <Box display="flex" alignItems="center">
                    <strong>{s.fromName}</strong>
                    <ArrowRightAltIcon
                      sx={{ mx: 1, verticalAlign: "middle" }}
                    />
                    <strong>{s.toName}</strong>
                    <Box component="span" ml={1}>
                      ${s.amount.toFixed(2)}
                    </Box>
                  </Box>
                }
              />
            </ListItemButton>
          );
        })}
      </List>
    </Box>
  );
}
