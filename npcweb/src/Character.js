import React from 'react';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableContainer from '@material-ui/core/TableContainer';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';

class SheetSection extends React.Component {
    render() {
        const cells = this.props.section.items.map((entry, i) => (
            <TableRow key={i}>
                <TableCell component="th" scope="row">{entry.item1}</TableCell>
                <TableCell>{entry.item2}</TableCell>
            </TableRow>
        ));

        return (
            <TableContainer component={Paper}>
                <Table aria-label={this.props.section.title}>
                    <TableBody>
                        {cells}
                    </TableBody>
                </Table>
            </TableContainer>
        );
    }
}

export class Character extends React.Component {
    render() {
        const sheet = this.props.data.sheet.map(s => <SheetSection section={s} />);
        return <div>{sheet}</div>;
    }
}