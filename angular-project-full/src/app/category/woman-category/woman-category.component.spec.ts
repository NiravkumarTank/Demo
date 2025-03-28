import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WomanCategoryComponent } from './woman-category.component';

describe('WomanCategoryComponent', () => {
  let component: WomanCategoryComponent;
  let fixture: ComponentFixture<WomanCategoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WomanCategoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WomanCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
