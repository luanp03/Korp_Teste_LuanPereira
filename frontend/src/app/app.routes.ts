import { Routes } from '@angular/router';
import { ProdutosComponent } from './produtos/produtos';
import { NotasFiscaisComponent } from './notas-fiscais/notas-fiscais';

export const routes: Routes = [
  { path: '', redirectTo: '/produtos', pathMatch: 'full' },
  { path: 'produtos', component: ProdutosComponent },
  { path: 'notas-fiscais', component: NotasFiscaisComponent },
  { path: '**', redirectTo: '/produtos' }
];
