import { useState } from "react";
import { Container, Box, Stack, Paper } from "@mui/material";
import logo from "./assets/coffeewise-logo.png";
import DailyCoffee from "./containers/DailyCoffee";
import OrderHistory from "./components/OrderHistory";
import BalanceDisplay from "./components/BalanceDisplay";

export const GROUP_ID = import.meta.env.VITE_GROUP_ID;

export default function App() {
  const [refreshKey, setRefreshKey] = useState(0);

  const triggerRefresh = () => setRefreshKey((prev) => prev + 1);

  return (
    <Container maxWidth="md" sx={{ my: 6 }}>
      <Stack direction="column" alignItems="center" spacing={4}>
        <Box
          component="img"
          src={logo}
          alt="CoffeeWise Logo"
          sx={{ height: 180, mb: 1 }}
        />
        <Section>
          <DailyCoffee triggerRefresh={triggerRefresh} />
        </Section>
        <Section>
          <OrderHistory refreshKey={refreshKey} />
        </Section>
        <Section>
          <BalanceDisplay refreshKey={refreshKey} />
        </Section>
      </Stack>
    </Container>
  );
}

function Section({ children }: { children: React.ReactNode }) {
  return (
    <Paper
      elevation={1}
      sx={{
        width: "100%",
        p: { xs: 2, sm: 3 },
        borderRadius: 3,
        boxShadow: "0 2px 8px 0 rgba(0,0,0,0.1)",
      }}
    >
      {children}
    </Paper>
  );
}
