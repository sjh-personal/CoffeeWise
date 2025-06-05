import { useEffect, useState } from "react";
import {
  Box,
  Typography,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  Paper,
  IconButton,
  Collapse,
  Stack,
} from "@mui/material";
import { KeyboardArrowDown, KeyboardArrowUp } from "@mui/icons-material";
import type { OrderDto, PersonMap } from "../types/dto";
import { fetchOrders, fetchPersonMap } from "../api/coffeewise";

export default function OrderHistory({ refreshKey }: { refreshKey: number }) {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [personMap, setPersonMap] = useState<PersonMap>({});
  const [open, setOpen] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      const [ordersRes, people] = await Promise.all([
        fetchOrders(),
        fetchPersonMap(),
      ]);
      setOrders(ordersRes);
      setPersonMap(people);
    })();
  }, [refreshKey]);

  return (
    <Box>
      <Typography variant="h6" sx={{ mb: 2 }}>
        Order History
      </Typography>
      <Paper sx={{ maxHeight: 400, overflowY: "auto" }}>
        <Table stickyHeader>
          <TableHead>
            <TableRow>
              <TableCell />
              <TableCell>Date</TableCell>
              <TableCell>Payer</TableCell>
              <TableCell align="right"># Items</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {orders.map((order) => (
              <OrderRow
                key={order.orderId}
                order={order}
                open={open === order.orderId}
                onToggle={() =>
                  setOpen(open === order.orderId ? null : order.orderId)
                }
                personMap={personMap}
              />
            ))}
          </TableBody>
        </Table>
      </Paper>
    </Box>
  );
}

function OrderRow({
  order,
  open,
  onToggle,
  personMap,
}: {
  order: OrderDto;
  open: boolean;
  onToggle: () => void;
  personMap: PersonMap;
}) {
  return (
    <>
      <TableRow>
        <TableCell>
          <IconButton size="small" onClick={onToggle}>
            {open ? <KeyboardArrowUp /> : <KeyboardArrowDown />}
          </IconButton>
        </TableCell>
        <TableCell>
          {new Date(order.date).toLocaleString(undefined, {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
            hour: "numeric",
            minute: "2-digit",
            hour12: true,
          })}
        </TableCell>
        <TableCell>
          {personMap[order.payerPersonId] || order.payerPersonId}
        </TableCell>
        <TableCell align="right">{order.items.length}</TableCell>
      </TableRow>
      <TableRow>
        <TableCell colSpan={4} style={{ paddingBottom: 0, paddingTop: 0 }}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <Box sx={{ margin: 2 }}>
              <Typography variant="subtitle2" gutterBottom>
                Items
              </Typography>
              <Stack spacing={1}>
                {order.items.map((item, idx) => (
                  <Box key={idx} sx={{ ml: 1 }}>
                    <Typography variant="body2">
                      <strong>
                        {personMap[item.consumerPersonId] ||
                          item.consumerPersonId}
                      </strong>
                      {": "}
                      <em>{item.description}</em> — ${item.price.toFixed(2)}
                    </Typography>
                  </Box>
                ))}
              </Stack>
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </>
  );
}
