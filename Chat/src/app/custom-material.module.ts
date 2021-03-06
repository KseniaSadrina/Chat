import { MatButtonModule,
  MatDialogModule,
   MatIconModule,
   MatTooltipModule,
   MatSelectModule,
   MatProgressBarModule,
   MatAutocompleteModule,
   MatBadgeModule} from '@angular/material';
import { MatInputModule } from '@angular/material';
import { MatListModule } from '@angular/material';
import { MatCheckboxModule } from '@angular/material';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSidenavModule } from '@angular/material/sidenav';
import { NgModule } from '@angular/core';

@NgModule({
  imports: [
    MatInputModule,
    MatInputModule,
    MatListModule,
    MatButtonModule,
    MatCheckboxModule,
    MatInputModule,
    MatFormFieldModule,
    MatDialogModule,
    MatSidenavModule,
    MatIconModule,
    MatTooltipModule,
    MatSelectModule,
    MatProgressBarModule,
    MatBadgeModule
],
  exports: [
    MatInputModule,
    MatInputModule,
    MatListModule,
    MatButtonModule,
    MatCheckboxModule,
    MatInputModule,
    MatFormFieldModule,
    MatSidenavModule,
    MatDialogModule,
    MatIconModule,
    MatTooltipModule,
    MatSelectModule,
    MatProgressBarModule,
    MatBadgeModule
],
})
export class CustomMaterialModule { }
