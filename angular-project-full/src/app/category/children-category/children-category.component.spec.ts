import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChildrenCategoryComponent } from './children-category.component';

describe('ChildrenCategoryComponent', () => {
  let component: ChildrenCategoryComponent;
  let fixture: ComponentFixture<ChildrenCategoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChildrenCategoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChildrenCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
