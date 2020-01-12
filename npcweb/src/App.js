import React from 'react';
import { AppBar, Toolbar, IconButton, Typography, TextField, Button, Container } from '@material-ui/core';
import MenuIcon from '@material-ui/icons/Menu';
import './App.css';

// TODO move this somewhere else :)
class NewCharacterEntry extends React.Component {
  createCharacter() { // TODO parameters
    fetch("https://localhost:5001/create", {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        Name: 'Qudp',
        Level: 1
      })
    })
    .then(r => r.json())
    .then(j => alert(j.id));
  }

  render() {
    return (
      <form>
        <Typography>New character</Typography>
        <TextField id="name" label="Name" />
        <TextField id="level" label="Level" />
        <Button onClick={() => this.createCharacter()}>Create</Button>
      </form>
    );
  }
}

function App() {
  return (
    <div>
      <AppBar position="static">
        <Toolbar>
          <IconButton edge="start" color="inherit" aria-label="menu">
            <MenuIcon />
          </IconButton>
          <Typography variant="h6">npc</Typography>
        </Toolbar>
      </AppBar>
      <Container maxWidth="lg">
        <NewCharacterEntry />
      </Container>
    </div>
  );
}

export default App;
