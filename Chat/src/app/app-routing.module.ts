import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { LogoutComponent } from './components/logout/logout.component';
import { TrainingDetailsComponent } from './components/training-details/training-details.component';

const routes: Routes = [
  { path: '', redirectTo: 'trainings/1', pathMatch: 'full' },
  { path: 'trainings', redirectTo: 'trainings/1'},
  { path: 'login', component: LoginComponent },
  { path: 'logout', component: LogoutComponent },
  { path: 'trainings/:id', component: TrainingDetailsComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
