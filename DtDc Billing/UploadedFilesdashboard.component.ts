import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { GlobalService } from '../services/global.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  nooftoken: number = 0;
  contractaddress: string | undefined;
  account: string = '';
  tronWeb: any;
  playerinfo: any;
  isConnected: any;
  tokenBalance: any = 0;
  userbalance: any = 0;
  data: any;
  balanceOfPNFT: any;
  balanceOfBNB: any;
  priceInfo: any;
  sellForm!: FormGroup;
  submitted: boolean = false;
  waitMessage: string = '';
  btnText = "Buy PNFT";
  balanceOfEAN: any;
  balanceOfBusd: any;
  contractInfo: any;
  claimable: any;
  useInfo: any;
  sidebar: boolean = false;
  loggedInAddress: string="";
  starting: string | undefined;
  hidecount: boolean = false;
  milliSecondsInASecond = 1000;
  hoursInADay = 24;
  minutesInAnHour = 60;
  SecondsInAMinute = 60;

  constructor(
    private cs: GlobalService,
    private formBuilder: FormBuilder,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.countdown("1647302410");
    this.getaccount();
    this.loggedInAddress = sessionStorage.getItem('userAddress') ?? "";

    let that = this;
    window.addEventListener(
      'message',
      function (e: any) {
        if (e.data.message && e.data.message.action == 'setAccount') {
          if (that.account != e.data.message.data.address) {
            location.reload();
          }
        }
      }.bind(this)
    );


    this.sellForm = this.formBuilder.group({
      from: ['', Validators.required],
      to: ['', Validators.required],
      type:['2',Validators.required]
    });
  }

  async getaccount() {

    this.cs.init();
    setTimeout(async () => {
      await this.cs.getWalletObs().subscribe((data: any) => {
        this.data = data;
      });
      this.getDetails();
      let that = this;
      setInterval(function () {
        that.getDetails();
      }, 5000);
    }, 1500);
  }

  connect()
  {
    localStorage.setItem('wallet','1');
    this.cs.init();
  }

  async getDetails() {
    // this.data.address = '0x29D021c94f27e2b875E53dd32F6A372955E75333';
    this.balanceOfPNFT = await this.cs.getBalanceOfPNFT(this.data);
   this.balanceOfBNB = await this.cs.getBalanceByAddress(this.data);
   this.balanceOfBusd = await this.cs.getBalanceOfUSDT(this.data);
    
    this.priceInfo = await this.cs.getpriceInUsdt();
    this.contractInfo  = await this.cs.contractInfo();
    
    this.claimable = await this.cs.getUserDividends(this.data);
    this.useInfo = await this.cs.UserInfo();
    
  }

  

  /***************sell form method*****************/
  async onSubmitSell() {
    try {
      this.submitted = true;
      
      if (this.sellForm.valid) {
        let fromAmount = this.sellForm.controls.from.value;
        var txStatus = false;
    
        var allowance: any = await this.cs.checkAllowance(
          this.data,
          fromAmount,
          this.cs.preSalePNFTAddress,
          this.sellForm.controls.type.value=="2"
        );

        if (Number(allowance.mainAmount) > Number(allowance.allowedAmount)) {
          this.btnText = "Approve Token...";
          var allowanceTxn: any = await this.cs.approveToken(fromAmount,this.cs.preSalePNFTAddress,this.sellForm.controls.type.value=="2");
          this.btnText = "Waiting for Confirmation...";
          await allowanceTxn.hash.wait(1);
        } else {
          txStatus = true;
        }
        this.btnText = "Transaction Initiated...";
        var status: any = await this.cs.exchangeToken(
          this.sellForm.controls.from.value,
          this.sellForm.controls.type.value
        );
        if(status.status)
        {
          this.btnText = "Waiting for Confirmation...";
          status.txn.wait(3);
          this.btnText = "Buy PNFT";
          this.toastr.success("Swapped successfully");
          this.sellForm.controls.from.setValue("");
          this.sellForm.controls.to.setValue("");
        }
        else
        {
          this.btnText = "Buy PNFT";
          debugger
           if (status.txn.code == 4001){
            this.toastr.error(status.txn.message);
         }
         else{
            this.toastr.error(status.txn.data.message);
         }
        }
      }
    }catch (e:any) {
      this.btnText = "Buy PNFT";
      
      if (e.hash.code == 4001){
         this.toastr.error(e.hash.message);
      }
      else{
         this.toastr.error(e.hash.data.message);
      }
      return false;
    }
    return true;
  }

  async calculatenooftokenSell() {
    let amount = this.sellForm.controls.from.value;
    let nooftoken = "0";
    nooftoken = (amount/(this.priceInfo / 1e18)).toFixed(4);
    this.sellForm.controls.to.setValue(nooftoken);
  }


  async claim()
  {
    try{
    await this.cs.preSalePNFTContract.claim();
  } catch (e:any) {
    console.log(e.code == 4001)
    
    if (e.code == 4001) {
      this.toastr.error(e.message);
    }
    else {
      this.toastr.error(e.data.message);
    }
  }
}

  menu_toggle() {
    this.sidebar = ! this.sidebar;
  }


  private countdown(time: any) {

    try {
      setInterval(() => {
        let timeDifference = time * 1000 - new Date().getTime();
        if (timeDifference > 0) {
          let secondsToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond) % this.SecondsInAMinute);
          let minutesToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond * this.minutesInAnHour) % this.SecondsInAMinute);
          let hoursToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond * this.minutesInAnHour * this.SecondsInAMinute) % this.hoursInADay);
          let daysToDday = Math.floor((timeDifference) / (this.milliSecondsInASecond * this.minutesInAnHour * this.SecondsInAMinute * this.hoursInADay));
          this.starting = daysToDday + "D : " + hoursToDday + " H : " + minutesToDday + " M : " + secondsToDday + " S";
        }
        else {
          this.hidecount = true;
        }
      },
        1000)
    }
    catch (e) {
      console.log(e)
    }
  }


}