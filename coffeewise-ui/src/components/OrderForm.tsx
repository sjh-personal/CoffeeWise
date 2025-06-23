import { useEffect, useState, useMemo } from "react";
import Box from "@mui/material/Box";
import TextField from "@mui/material/TextField";
import MenuItem from "@mui/material/MenuItem";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Snackbar from "@mui/material/Snackbar";
import type { OrderItemDto, PersonDto } from "../types/dto";
import { submitOrder as submitOrderApi } from "../api/coffeewise";
import Alert from "@mui/material/Alert";

export default function OrderForm({
  members,
  presentMap,
  recommendedPayer,
  onOrderSubmitted,
}: {
  members: PersonDto[];
  presentMap: Record<string, boolean>;
  recommendedPayer: PersonDto | null;
  onOrderSubmitted?: () => void;
}) {
  const presentMembers = useMemo(
    () => members.filter((m) => presentMap[m.personId]),
    [members, presentMap]
  );

  const presentMemberIds = useMemo(
    () => presentMembers.map((m) => m.personId),
    [presentMembers]
  );

  const [items, setItems] = useState<
    { consumerPersonId: string; description: string; price: string }[]
  >([]);
  const [payerId, setPayerId] = useState<string>("");

  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbarError, setSnackbarError] = useState(false);

  useEffect(() => {
    if (
      recommendedPayer &&
      presentMemberIds.includes(recommendedPayer.personId)
    ) {
      setPayerId(recommendedPayer.personId);
    } else if (!presentMemberIds.includes(payerId)) {
      setPayerId("");
    }
  }, [recommendedPayer, presentMemberIds, payerId]);

  useEffect(() => {
    setItems((prev) =>
      presentMembers.map(
        (m) =>
          prev.find((it) => it.consumerPersonId === m.personId) || {
            consumerPersonId: m.personId,
            description: "",
            price: "",
          }
      )
    );
  }, [presentMembers]);

  useEffect(() => {
    if (payerId && !presentMemberIds.includes(payerId)) {
      setPayerId("");
    }
  }, [payerId, presentMemberIds]);

  const handleChange = (
    idx: number,
    field: "description" | "price",
    value: string
  ) => {
    setItems((prev) =>
      prev.map((it, i) => (i === idx ? { ...it, [field]: value } : it))
    );
  };

  const isValidPrice = (val: string) =>
    val === "" || /^\d*\.?\d{0,2}$/.test(val);

  const canSubmit =
    payerId !== "" &&
    items.length === presentMembers.length &&
    items.every((it) => it.price !== "" && !isNaN(Number(it.price)));

  const submitOrder = async () => {
    if (!canSubmit) return;

    const payloadItems: OrderItemDto[] = items.map((it) => ({
      consumerPersonId: it.consumerPersonId,
      description: it.description.trim(),
      price: parseFloat(it.price),
    }));

    try {
      await submitOrderApi({
        payerPersonId: payerId,
        date: new Date().toISOString(),
        items: payloadItems,
      });
      setItems([]);
      setSnackbarOpen(true);
      if (onOrderSubmitted) onOrderSubmitted();
    } catch (err) {
      console.error("Failed to submit order.");
      setSnackbarError(true);
    }
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Submit Today’s Order
      </Typography>
      <TextField
        select
        label="Payer"
        value={presentMemberIds.includes(payerId) ? payerId : ""}
        onChange={(e) => setPayerId(e.target.value)}
        sx={{ mb: 2, minWidth: 240 }}
        required
      >
        {presentMembers.map((m) => (
          <MenuItem key={m.personId} value={m.personId}>
            {m.name}
            {recommendedPayer && m.personId === recommendedPayer.personId
              ? " (recommended)"
              : ""}
          </MenuItem>
        ))}
      </TextField>

      <Grid container spacing={2}>
        {items.map((it, idx) => {
          const m = members.find((mm) => mm.personId === it.consumerPersonId)!;
          return (
            <Grid key={it.consumerPersonId} size={{ xs: 12, sm: 6, md: 4 }}>
              <Typography variant="body1" sx={{ mb: 0.5 }}>
                {m.name}
              </Typography>
              <TextField
                label="Description"
                value={it.description}
                onChange={(e) =>
                  handleChange(idx, "description", e.target.value)
                }
                fullWidth
                size="small"
                sx={{ mb: 1 }}
              />
              <TextField
                label="Price"
                value={it.price}
                onChange={(e) => {
                  const val = e.target.value;
                  if (isValidPrice(val)) {
                    handleChange(idx, "price", val);
                  }
                }}
                fullWidth
                size="small"
                type="number"
                inputProps={{
                  step: "0.01",
                  min: "0",
                  pattern: "\\d*(\\.\\d{0,2})?",
                }}
                required
              />
            </Grid>
          );
        })}
      </Grid>
      <Button
        variant="contained"
        onClick={submitOrder}
        sx={{ mt: 2 }}
        disabled={!canSubmit}
      >
        Submit Order
      </Button>
      {!canSubmit && (
        <Typography color="error" variant="body2" sx={{ mt: 1 }}>
          All present people must have a valid price entered to submit.
        </Typography>
      )}

      <Snackbar
        open={snackbarOpen}
        autoHideDuration={3000}
        onClose={() => setSnackbarOpen(false)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={() => setSnackbarOpen(false)}
          severity="success"
          elevation={6}
          variant="filled"
        >
          Order submitted!
        </Alert>
      </Snackbar>

      <Snackbar
        open={snackbarError}
        autoHideDuration={3000}
        onClose={() => setSnackbarError(false)}
        anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
      >
        <Alert
          onClose={() => setSnackbarError(false)}
          severity="error"
          elevation={6}
          variant="filled"
        >
          Error submitting order.
        </Alert>
      </Snackbar>
    </Box>
  );
}
