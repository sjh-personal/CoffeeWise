import { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Paper,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  CircularProgress,
} from "@mui/material";
import type { BalanceSummaryDto } from "../types/dto";
import { fetchBalances } from "../api/coffeewise";

export default function BalanceDisplay({ refreshKey }: { refreshKey: number }) {
  const [balances, setBalances] = useState<BalanceSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      try {
        setBalances(await fetchBalances());
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [refreshKey]);

  if (loading) return <CircularProgress size={32} />;

  return (
    <Box>
      <Typography variant="h6" sx={{ mb: 2 }}>
        Current Balances
      </Typography>
      <Paper sx={{ mb: 2 }}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell align="right">Paid</TableCell>
              <TableCell align="right">Owes</TableCell>
              <TableCell align="right">Balance</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {balances.map((b) => (
              <TableRow key={b.personId}>
                <TableCell>{b.name}</TableCell>
                <TableCell align="right">{b.paid.toFixed(2)}</TableCell>
                <TableCell align="right">{b.owes.toFixed(2)}</TableCell>
                <TableCell
                  align="right"
                  sx={{ color: b.balance < 0 ? "error.main" : "success.main" }}
                >
                  {b.balance.toFixed(2)}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </Box>
  );
}
