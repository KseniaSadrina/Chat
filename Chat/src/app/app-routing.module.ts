import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { TrainingDetailsComponent } from './components/trainings/training-details/training-details.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { HomeComponent } from './components/home/home.component';
import { TrainingsListComponent } from './components/trainings/trainings-list/trainings-list.component';
import { TrainingsComponent } from './components/trainings/trainings.component';
import { AddTrainingComponent } from './components/trainings/add-training/add-training.component';
import { RegisterComponent } from './components/register/register.component';
import { AuthGuard } from './helpers/auth-guard.service';

const routes: Routes = [
  { path: '', redirectTo: 'home/trainings', pathMatch: 'full' },
  { path: 'home', redirectTo: 'home/trainings', pathMatch: 'full' },
  { path: 'home',
    component: HomeComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'notfound', component: PageNotFoundComponent },
      { path: 'trainings', component: TrainingsComponent,
        canActivate: [AuthGuard],
        children: [
          {
            path: ':id',
            canActivate: [AuthGuard],
            component: TrainingDetailsComponent
          }
        ]}
    ]
  },
  { path: '**', redirectTo: 'home/notfound'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
