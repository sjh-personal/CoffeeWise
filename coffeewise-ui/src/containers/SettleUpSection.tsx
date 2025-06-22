import React, { useState } from "react";
import {
  Box,
  Typography,
  RadioGroup,
  FormControlLabel,
  Radio,
  Divider,
  Button,
  Stack,
  Paper,
} from "@mui/material";
import SimplifiedSettlements from "../components/SimplifiedSettlements";
import PairwiseSettleUp from "../components/PairwiseSettleUp";

type PendingPrefill = {
  from: string;
  to: string;
  fromName: string;
  toName: string;
};

export default function SettleUpSection({
  refreshBalances,
  refreshKey,
}: {
  refreshBalances: () => void;
  refreshKey: number;
}) {
  const [mode, setMode] = useState<"simplified" | "manual">("simplified");
  const [prefillFrom, setPrefillFrom] = useState<string | undefined>();
  const [prefillTo, setPrefillTo] = useState<string | undefined>();
  const [pendingPrefill, setPendingPrefill] = useState<PendingPrefill | null>(
    null
  );

  const handlePrefillClick = (
    from: string,
    to: string,
    fromName: string,
    toName: string
  ) => {
    setPendingPrefill({ from, to, fromName, toName });
  };

  const confirmPrefill = () => {
    if (pendingPrefill) {
      setPrefillFrom(pendingPrefill.from);
      setPrefillTo(pendingPrefill.to);
      setMode("manual");
      setPendingPrefill(null);
    }
  };

  const cancelPrefill = () => {
    setPendingPrefill(null);
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Settle Up Options
      </Typography>

      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        CoffeeWise usually recommends who pays next daily. Here, you can either
        settle debts manually between two people or use simplified settlements
        to clear balances efficiently.
      </Typography>

      <RadioGroup
        row
        value={mode}
        onChange={(e) => setMode(e.target.value as "simplified" | "manual")}
        sx={{ mb: 3 }}
      >
        <FormControlLabel
          value="simplified"
          control={<Radio />}
          label="Simplified Settlements"
        />
        <FormControlLabel
          value="manual"
          control={<Radio />}
          label="Manual Settle Up"
        />
      </RadioGroup>

      <Divider sx={{ mb: 3 }} />

      {mode === "simplified" && (
        <>
          <SimplifiedSettlements
            onSelectPair={handlePrefillClick}
            refreshKey={refreshKey}
          />
          {pendingPrefill && (
            <Paper
              elevation={0}
              sx={{
                mt: 3,
                p: 2,
                borderRadius: 2,
                bgcolor: "background.default",
                border: (theme) => `1px solid ${theme.palette.divider}`,
              }}
            >
              <Typography>
                Start manual settle-up between{" "}
                <strong>{pendingPrefill.fromName}</strong> and{" "}
                <strong>{pendingPrefill.toName}</strong>?
              </Typography>
              <Stack direction="row" spacing={2} sx={{ mt: 1 }}>
                <Button
                  variant="contained"
                  size="small"
                  onClick={confirmPrefill}
                >
                  Yes
                </Button>
                <Button variant="outlined" size="small" onClick={cancelPrefill}>
                  Cancel
                </Button>
              </Stack>
            </Paper>
          )}
        </>
      )}

      {mode === "manual" && (
        <PairwiseSettleUp
          refreshBalances={refreshBalances}
          prefillFrom={prefillFrom}
          prefillTo={prefillTo}
        />
      )}
    </Box>
  );
}
