using System;

using Xunit;

namespace PriceAlerts.Common.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("not a number at all")]
        [InlineData("contains spaces                           ")]
        [InlineData("contains non punctation characters $+=±£¢¤¬¦²³¼¾~<>")]
        public void ExtractNumber_ContainsNoNumber_EmptyString(string original)
        {
            var numberString = original.ExtractNumber();

            Assert.Equal(string.Empty, numberString);
        }

        [Theory]
        [InlineData("12")]
        [InlineData(":;!?,.()[]{}*\\\"#/%&_-@")]
        [InlineData("12,34,45")]
        [InlineData("12,345,678.90")]
        public void ExtractNumber_ContainsOnlyNumberAndPunctation(string original)
        {
            var numberString = original.ExtractNumber();

            Assert.Equal(original, numberString);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("this is a number 12", 12)]
        [InlineData("12,345,678.90", 12345678.90)]
        [InlineData("12 345 678,90", 12345678.90)]
        [InlineData("12 345 678.90", 12345678.90)]
        [InlineData("$ 123.45", 123.45)]
        [InlineData("£ 123.45", 123.45)]
        [InlineData("¢ 123.45", 123.45)]
        [InlineData("123.45", 123.45)]
        [InlineData("123,45", 123.45)]
        [InlineData("123.00", 123)]
        [InlineData("123,00", 123)]
        [InlineData("123", 123)]
        public void ExtractDecimal(string original, decimal result)
        {
            var number = original.ExtractDecimal();

            Assert.Equal(result, number);
        }

        [Fact]
        public void IsBase64Url_IsBase64JpegImage_ReturnsTrue()
        {
            var imageUrl = @"\ndata:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAoHBwgHBgoICAgLCgoLDhgQDg0NDh0VFhEYIx8lJCIfIiEmKzcvJik0KSEiMEExNDk7Pj4+JS5ESUM8SDc9Pjv/2wBDAQoLCw4NDhwQEBw7KCIoOzs
                7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozv/wAARCAEsAPIDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaE
                II0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwE
                AAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYa
                HiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDrvif401PwVp1jc6ZBazNczNG4uUZgABkY2sK87/4Xn4t/6B2kf9+Zf/jldL8fP+QJpH/X23/oNeNYpiZ6B/w
                vPxb/ANA7SP8AvzL/APHKP+F5+Lf+gdpH/fmX/wCOV5/ijFFhXO/b46+LEGTp2kY6f6mX/wCOUg+O/isqWGn6QQOv7mX/AOOVy/hjVbbRdQu57sNtns3gjkVNzROSpDDBBHAPIOea2LnxPoFxq2namLVo/sFyZJLcWq4uFbZk5z/sscNnryckmiw7mifjr4s
                UEnTtI+Xr+5l/+OUn/C9/FeAf7P0fk4H7mX/45WHY65o2l3ulvHbz3S6bazReftWMyO/mkHac8gyIA2eNucHAFW4vFmieZJLcWE0j3tvHa3TGOMyJGEIdt+PncsVYNhSPLUUWC5oj47+Kz00/SD/2xl/+OU7/AIXn4tzj+ztI/wC/Mv8A8crmNX8RPfNatYI
                lk6Rlrt4own2i4bh3I9CAvHABLYAyazJ2099Iht4LSWO/WRjNcNNlJEIG0BccYwe/fv2LBc7c/HjxUDg2Gkdcf6mX/wCOUH48eK1JB0/SMjr+5l/+OVRh8YaXbxRq8Rvbc2tvbNZS2y7bcKqrO6Nn7zAOMjBO/J6UWfivSLVUt4TNbWlrPCUVbVGN3AikOkg
                zj52LMQcj5vagLmj/AMLz8W/9A7SP+/Mv/wAcpD8dfFg/5h+kf9+Zf/jlcjFptu1jJreou1nYzTutpaw8y3BByQmeFRcgFznngAnOG2k895qFvp+lWFrby3MqxRb1EjZJwCXfOPqMUBdnYj45+Lj00zST9IJf/jlH/C8/Fw66dpA+sMv/AMcqDxL4J1jSNQi
                0+w12a9mKAzGWXyUDHoqjcf8ADkVh6jpL+H9QjsPF9nfWrzLuWaC6SQqvTdt5DfTcKNBXOj/4Xn4t/wCgdpH/AH5l/wDjlH/C8/Fv/QO0j/vzL/8AHK5jxX4Zm8IaxHYXE6XkU8IngnQbC6HI5U/dOQeOazp7B0sI9QglEto8nlM2MNDJjOxx2yMkHocH0Io
                C7O3/AOF6eLcE/wBnaRgf9MZf/jlN/wCF8+Kv+fDSP+/Mv/xyuUjvtIh8R6bqFrp8sNhayQPcQSyCUyFcFz0HXB46c9ug6CDxnpMCMl7E2rTs7Mt7PaIHjUshVcEncEKlhk4+bHAoHct/8L48VY/5B+kf9+Zf/jlL/wAL38V8/wDEv0jgZ/1Mv/xys6Lxfp1
                rJYNG1zdzwXUEl5dzRJvuIl3+YnqQQ6jDE9OuAKLLxbpEcNjDf2sty9rcRXkty0SM00wcmQHPJUxsExnH7tTgZoC5pn46eLV66dpHJx/qZf8A45TD8ePFYODp+kcf9MZf/jlcnpdzpuj6nPK+7U7SS0lhCvF5e53QqOMkgAnqDmuhi8YaQiWQiheyhtmja5s
                ktlkS9RYo0MTEnkEo/LZ+/nrRYLl7/hefi7/oHaR/35l/+OUf8Lz8W/8AQO0j/vzL/wDHK89VdoOeOeBnOKdiiwrnoH/C8/Fv/QO0j/vzL/8AHKP+F5+Lf+gdpH/fmX/45Xn+KMUWC56B/wALz8W/9A7SP+/Mv/xyu2+GPxC1jxpqF/b6nbWUK2sSsv2dHUk
                kkc7mPpXhOK9P+Av/ACG9b/64R/zNDQ0z22iiikM8r+O5gGi6T55kC/am/wBWBn7vvXjn2jSv+nz/AL5X/GvXvj//AMgLSf8Ar6b/ANBrxGncTRofaNK/6fP++V/xo8/Sv+n3/vlf8az6KLhY0hPo38X2/wDBU/xqVJ/DX/LUar/wHy6yKe0QWBJfNRi7Mpj
                GdyYxyeMYOeMHsaLisbsc3gr/AJapr3/AWhqws3w8/jTxL+DQVy+KMUXCx1qz/DT+KLxT+DwVItx8Lh9638Un/gcH+Ncdijg0rjsdwt38JgOdO8TMfUtF/wDF1FNcfCpx+7tfFCewaDH6k1xmB6UYHpTuBv6zPo1+9smhS3ggs4fKSHUCvmEFmY7SvB5Y8cH
                61Dp2jnVILmRr+zsUgwP9KfaZGOcBR+HJ9xWKQDTt78fNnHTdzRcVjoYNO1GCX7J/bSIvGBHqGxMHP19M9O4rQsrAaNcyzSW+krfW5BU6ndNMwbGcqi4UnBBG4H9OOVivEjGJLC2mH+1vH8mFWF1aBANmhaaCOjMJW/QuR+lAGhrQ1fVdbSS6uDq+p3fH+ju
                JicEjChRhRwflFXIP7F0LSrjSddmnnuL6aKS4j09kY2yx7sKWPylyWOQM4x1ySBhTa9qk1s9qtwLa2k4eC1RYUcf7QUDd+OazwoFFwOwE/wALxwYPFRH+/BU6XXwoUfNZeJ3+rQ/0YVxOKMD0pDO2kuvhOw+Sx8Txn/ZaH+rGqkk3w2z+6i8T/wDAngrlMD0
                oxTuKx0bz+Bf+WcfiD/gTw/4VA8/hL/lmms/8CMVYfFFFwsazXHh7+Aan+Pl1GZ9F7C//ABCf41m0dKLhY0ftGkdvtv8A3yn+NJ9o0r/p9/75X/Gq09jcW1nbXkqoIbsMYSJFYnacHKg5Xn1Az2qCi47Gh5+lf9Pv/fK/416d8CWtW1rWTbmbmCP/AFgA/iP
                pXkNer/AD/kMaz/1wj/8AQjRcLHuFFFFIZ5N8f/8AkBaT/wBfTf8AoNeI9RXt3x//AOQFpP8A19N/6DXiNACsxdy7HLMcnjFJRRQIKKKlklhe2hjS2WOWPd5kwYky5PGQeBgccde9AEXce9FOWWRI3jViEkxvX+9jpTaAHARiMPvJkDf6vbwV9c5/SnXMwub
                qW4WCK3EjlhDECETPZQSTj8ajqWK0uZ7ae6igeSC22+fIq5WPccLk9snigCKiiigAoopQMqTuAI6A9T9KAEpWAViFYMAeGHQ0lFABQQykqylWHBB4IpyO0UqSxna6EMrDsR0NT6jqN3q+ozajqE3nXc7bpZNoG4/QYA/CgCtRRRQAUqglhjGRz8xwKSigAJy
                ScYyeg7UUpABwGDcZyP5UlABRSZ5x6Uvt3oAO5PrRVqfTb63tPtctjPDbrKbd5XQ7fNHJXOODjt7VVoAK9X+AH/IY1n/rhH/6Ea8or1f4Af8AIY1n/rhH/wChGgD3CiiigZ5N8f8A/kBaT/19N/6DXiNe3fH/AP5AWk/9fTf+g14jQAUUUUCCiiigCS3t57y
                5jtbaJpp5mCRxoMszHoAKjPDFTwQcEelHI6HB9RRQAUmOQfSloJxQAUVLcWs9p5X2hNhmjWWPkHch6HioqACiiigAooooAKKKKACiiigCzp0NpPqUEGo3LWdo7gTTrGXMa+u0cmoJVjWeRYnMkSsQjkYLLng47fSkYlyWcliepJyTSUAFKACG3HGBxxnJpKK
                AJWu7h4IrdpT5UAZY1HGAxyw98n1qKiigB/nzm2FsZpDbq28QljsDYwWx0zjvTKKKAFXbn5gSMHp1z2/WvVfgB/yGNZ/64R/+hGvKa9X+AOP7Y1nH/PCP/wBCNAz3CiiigDyb4/8A/IC0n/r6b/0GvECcDpmvb/j/AP8AIC0n/r6b/wBBrxGgC3qVjDYPbrD
                fwXomt0mYw5xEzdY2yPvDvVSgDFFAhSd2PlAwMcd/ekopQcBhtByMc9vcUAJRRRQAUUVLCLXyp/tBlEuwfZ/LAKl9wzvz0G3d074oAZFI0LFkCkkFfmUMMEYPB789e3UU2j60UAORAyuxkVCi5CtnL8gYGB15zzjgGlggmuriO3t4nlnlYLHGgyzk9AAOppl
                HPUEg9iKADoSO460E+2aKKALeq2UOm6lLaW9/BqESYxcwZ2PkA8Z9M4/CqlAGBiigBWO5s7QvsOlJRSg4UrtHJ6nqP8/0oASiiigAooqVfs32OXeZftW9fKCgeWUw27d3znbjHvntQBFRRRQA4IDEz+YoKkAIc7mznkcYwMc5Pce9Ot7ae7l8m2heaTazbI1
                LHABJOB2ABP4VHRkjoSD7UAGR2r1f4Af8hjWf+uEf/oRryivV/gB/yGNZ/wCuEf8A6EaBnuFFFFAHk3x//wCQFpP/AF9N/wCg14j2x617d8f/APkBaT/19N/6DXiNAC4IUN2PTmkoqU20y2SXpC+Q8jRKd4zuABPy5zjDDnGPyoERUUUjHAoAkhhmuZ0gtoZ
                J5pDtSONSzMfQAcmt0+BtZhCi+m07TZGUMIr2/iikx/ulsj8atvdSeDvDlnHYbotZ1eD7TPdrw8FuSQiIexbG4kYPIFc/pNjHqd6yXNz5QwXLMfvevJ6cZ55rnU51NYu0fvb/AK/EtR6Gr/whd5/0GdB/8GsP+NJ/whd5/wBBnQf/AAaw/wCNZs9hBFPMIJ1
                ngik8sTDoT/nPPfFXrXSoXVUKBpjyB2xSlzxWsvwOzD4GpXdolgeBNTbhdT0Vs+mpxf41PJ8NfEEMMU802mxRTDMTvfRhZB6qc810/hfwvAbZ9V1gGHTLQ/vNq4adu0af1P8ALqM7xZq93q1411KixKFCxwL92GMcKorm9rW/mX3f8E9ZZLTu05vTf16Jd31
                fYwj4E1JeuqaIPrqcX+NMPgq9HXWdBH11SH/Gs64YM5UgKQOQWx/OqUqxkE7ju7ZHH866oRrNXcvwPGr0YU3aLuaup+GL7SdNXUpLmxurVphAZLO6SYK5BYA7TxwDTtM8K6jq2mHU457K1tPNMSyXl0kIdgATjceeoqxHx8Lrj/sNR/8Aol66LX/B2rzaRoW
                kwBEgtLJZGVupnlJZ8/TgfhWDrzXut63avbov6SMqdF1JWirnPHwPqAj8z+1tD2Zxu/tOLH55ofwNqEQUy6voce4bhu1SIZHr16VX0e2vrDXltVtHkf5vNiMSyBlQFmYK3ytt27sH0qHVlu72OTU3Ecgll/fGHlUbt9AefbtW0VVclHn/AAK9kuVt7oq6ppt
                1o2pTadfIEuISNwVgwIIBBBHUEEH8aqE4HNdL4nVr3QdC1k/60Qtp9yMch4T8pPuY2X8q5l+gram5OPvaNXT9VoznZ0s3gPVLWTyrrUdGtpgAWim1KJHXIzyCcjg0weCr0nA1nQSfQarD/jW7470F7zxlI8sv2eG4ijZJGXIY7FBFZen+GLLWNNFtpcrLrNt
                I/mCVv3dwAAQqejf1I55FbrCVvZwqyn7sknsuyv16Amne3Qr/APCEX/8A0F9C/wDBpD/jT0+H+tTHbaXWlXcp5EUGows5+g3U/UPDeox3INvpd80LqHU/Z3zgjPp1qbTopLecqIXingILRuhDr3yR1FewsmpyTcMQnpfZP79TB1mvsnNahpt/pF2bTUrOa0n
                HOyVCpI9R6j3FV69cv9Ps73RoJJnEuh3LiGePq2mTscLPHnpGWwGXgcnA5wPKb2zm07ULmwuQBNbStFIAcjKnB/lXhSU6dR0qm67bP+uqNYyUo8yIm2fLsLHj5tw7+3tSUD3opjCvV/gB/wAhjWf+uEf/AKEa8pG3axJII+6AOv19K9W+AH/IY1n/AK4R/wD
                oRoA9wooooGeTfH//AJAWk/8AX03/AKDXiNe3fH//AJAWk/8AX03/AKDXiNABSsrK5VwVZeGBGCD6Un41dXQ9adFddHv2VhkMLZyCPXpUuUY7uwFMnOOAMDHFNYZWr/8AYOuf9Aa//wDAZ/8ACj+wdb/6A1//AOAz/wCFT7Wn/MvvCzNnxSgv7DQdYhU+Tca
                elqzZB2Sw/Iy47cBSM9Qa5VV/eBTwc45rrfD39o2EE2k6zomozaLdsGlUWz77eQcCaMkcMO/qODWdr3hybR9WurOSdHeFQ6kAjzVI3AgdjtIJB6VhQnGD9lf09P8AgG8Y88fd3R1FprU0vgyGbQtO06WTSYyNRs7m0WWQck/aFY8sp/iH8OPTmsdPiJqqHdH
                pmiqfVbBBUngDTdavPEtvLpm6CdDuMjZChe+7/Zx19c4qt8QNHs9F8WSrpjKbC7QXFvs+6FbIIHtuVse2KwjQo+2cJxve7T/NG1WFWlCM7tX/AKX39DtPAfjvWPE2unSdVFqbBbaSTy44AoUr0IrG8SIoupArZBz+IrI+HsjQ6vqkq/eTSblh+Cisy51GSZ8
                byWPXmoVCKryUFZaHqZXiY0qU3N3udHe6knhzwroclrpOmTzXv2hp5Lu1WZmKuAvJ6DHasr/hOLr/AKF/w9/4LI6m8VMW8IeFieuy6/8ARgrlq1w2Hp1KfNJXd5f+lM8bESftZWOv0nVrvxjq2m+H57LT7Sw+1i5njs7ZYQwRSWJx/sbhXpNx/aXiCxuJhff
                YlztV4kzLgsSASeMYOOACK4H4ZWaoup6rL8u5UsYCVyC0hy2PcKp/76r2i20NE01rdQCJlw+RkAVyVFau1SWkf+Hf6fcdmGcYUXOe7eny3OCubi08JZ114lmSyMdtCX5eZpGHmHI6ERq+PrXI3/hW68Oapcy2t7GiK8vloyb/ADI8jAYH5TuVlOOa7P4heDb
                /AFW0tLHSZ7WCxtWaW4a5n2ZcgBe3QLnH1rC1u8SLT9J0e5u7C4le2W3nmtZRJ5UsZxGzH0K7R+BrXL7SxCV9ZXTXa236/eKtV55yk1pbQyIIBqvhDWrOOOJNoXUYI4ZCUDxHbKQG5HyNnBz93jjpwzHKCu202dfDnijTobqNFtoJGiuAwxvR8o5b1+U5rld
                a02TR9ZvdMlJLWs7xZIxuAOAfxHNe7iqE6Fe0/tJS/R/kvvPLUlJXR6z4oS4vNXGj3jFre4jjks0I27m2qNob0OD+Ix3qho+q2UfjHTp7029hcKRBMsMXlREAkKWbPzN0ySB0rq7iKy8bfa/Dc6fZ9R0tUa1uQOGBRSQT65P9fWuB17w9eWiSIbOae4ZsPuB
                EhbvjBOcivYoUaeKy2NGXuzUF+K3+a/E5qVR060tdGyqdJ+IhdwNYmzkkj+2o/wD45xV661690PSdPt9W1RLrWEeTzWSQT7LfgrDKwOHy4JAycD64rkLJYP3oVgC6HB7k9fy4xUlrLEj7TGu1+Gz0x7+1ceDyPmSquaTi3oo26d7v7jeVble1zoLPUrnVjcW
                Gi2k063n7lY3+ZUDcHeQPrzxjHtWD42uIrrxzrMsGPLN24BB4ODgn8SCa7vUrnUfB+nG10rTp73U7qEFL21t2a2t0I/5ZNj52x34Az35z5suga2Bxo1+ff7M/+FebiMXRr1U4NcsU1fq299+isrd9S0nq2rXKVBOTk96vf2FrYU50a/C9T/oz/wCFUaiMoy+
                F3CwV6v8AAD/kMaz/ANcI/wD0I15ZbpA7sLmZoVCMVZY9+5gPlXGRgE8Z7ehr1T4Bbf7Z1jbnH2ePr65OaoD2+iiigDyb4/8A/IC0n/r6b/0GvEa9u+P/APyAtJ/6+m/9BrxAkDGaAEf7oOQc/pXd+OvEGrWvja8tIdZ1C0gVIAiQXDKiZhQ8LuAHJJrhXJl
                Jfuxye1dr4hl8H+I9al1aTxDd2jTpGDB/ZxfYVRV+9vGfu1y1lH2sZSjdWfS/VFRdloYreJtdBITxLfPwSD9rmB/njNNXxTrxbe3iDUgc/c+2SYP61bGleDR08V3n/grP/wAXTjp3g8rt/wCEruwPT+yj/wDF0c1D+T/yV/5Gil3LujeNNcsLtJ21a7mMZyR
                NM0in2IY4IrT8QW2leIby2u9Lilt7u4IM9rKx4dv4kYn5lPfPT6dMGKx8GxOGHim9IHQf2Yf/AIupBF4X+0NcN4y1BpGYuz/2acsT1531z2pxbcE1/wBuv/I9elmFNRipxv8A117+XVHU6/qdr4O0dvDmkSLNdzqFvrmI7vNJ6RJ/sjOD69PUVyPj59mrWGm
                uwafTdPiguCCCBKdzsMj0L4/Cr934t0DTGjl8O6bJcajGmFvr1QEjb++kWSN3cZOAe1cU8rSyvJLI0kkjFndjksTySTWlCnKU1OSslt/X9XZy4/Fqr7kHdb+r7/ol0Wh0fgg4utcP/UFu/wD0CsC0y8oHcmtjwjqNhpuoXo1OWSG3vbCa182OPeULjAOM81b
                t7Dwbbvu/4Sm8Yj/qFkf+z1UpclSbaetujf5HJQlFSjzPQk8Wrs8J+F19Euf/AEYK5NuFrp/GGq6Rfafo9jpNzNcrYLKJJZYvLyXYEYGT71zUPlSXESzMyw7x5jKMkLnkj8KvCJxormVnd/i2xYmUZVpOO1z1zwfpUlpoWjx/ZWkVEbUJ8H77yf6tQO7bFHH
                +1XpFprcRs4mkYebOQEiGNxJ6KPw5ryu+8Y+HbiZ5LTxDdWWHQ24i04kQqoAA+/ycKoz6DpVfUPGegJa3t9bahdXurS2rQQhrbykR3+VpByQDgn/61eUlW+KKfM/J9e/kjulPDujGN9l+J6JrElzqFtdosdu6EhIl89MuO7dRjsK8j17wh4hijuL7+zCtrAp
                eVknjbCjqcBs4ri9nqa1vDGrRaH4htru5QyWjbormMc7onBV+O/BJ+oFenRjicMnKMlJ9rf8ABOSWJcqfs7WRLf38uqW0c9wd8yfKznqR2q543Bvl0jX+SdSslSZieWni/dufxAU/jUq2PgtFZf8AhKLxgTx/xLD/APF0urXXh1fBx0mw1Se/uUvRcws9mYt
                gK7XXO48HCn8K9rM8bh8VGlKnfmWj91rR77q29mcVODjddDsrrxGdM8Z3cQm8sxGIg/8AbNa7K2uNP8aTWeqWcqpe6dMrXEWfvL6j1/yK8o1i68H63rMurPr93bvOELQHTi+0hQpGd4z0q94X13wt4X1RLyDxHezIuQ0X9nFd6nqCd59u3aoeLw9TLlB3VVQ
                5V7stfdtZ6W1InS99SXcy5IYPGFtLqulxJBrUAMl7Yx9Jx3liHr/eX8R785HKVbIxz61Wtry4sL5L6zle3niffG6HBU100mseGPEDm51aG60nUX5lms4xLBK3dzGSCpPsSO/etMBmDwsfZVU3Do92vJ9WuzWq2LnDm1RJofiO/wBPX7Pa6hcwRE5EaSsFz9M
                1n3XizxBPeTSprmpIruSqi7kAA7d6uC28GKCB4nvenUaYf/i6QWXgr/oZrz/wWH/4uvSxWYZZWaajru/cf+RnGE1/w5d8EeIdcufGmlwXGs380Uk2HjkunZWGD1BODXEH/Wv/ALxrudCufBeh61baoNfvbhrV94i/s4rv4Ixnecda4Y5LMxGMnNeBXdCWJc6
                C93lXRx1u+6Rsr8uorMzsWdizHqSck16t8AP+QxrP/XCP/wBCNeUA5GRXq/wA/wCQxrP/AFwj/wDQjUDPcKKKKAPJvj//AMgLSf8Ar6b/ANBriPhL/Z154p/sbUtHs9RjvlJElwgZodiO3y59TjP0Fdv8f/8AkBaT/wBfTf8AoNeReH9dvfDOsRatp4jNzEr
                KvmruX5lKnjI7GgCvrF/HqWq3F7BZQWEcrZFtbjCR8AcD8M/jXsPiLwhotpD4mv4rbSdq6QjQWcQHnWr45kK4+XPr3rxRgCSW4yc16JJN8RNXN/qC6CrrrNklvI6RYBixwV+bg4PWonUhT1nJL1BK5ifC6wtNU+IOnWd/bR3VvIJd8UqhlOImIyD7gVD46nt
                pdfEFtaaNCtvGFMmjPuglz8wOfUZwfpV/w/4Z8d+Gtag1bT9Bm+0wBgnmqCvzKVORkdiayfEOqap4j1qK1utNtra9hf7MttZ2/l5cucgju244pQrU535ZJ27MLMw8D0r0P4Xadp91ofiK8vLTR5prU23kyawdsEe5mBy2OM8fjiuc/wCEB8XH/mAXf/fI/wA
                a0rLw54607QNT0iLw/KbXUvLNwWQFv3Z3Lt59fY1H1qh/z8X3ofK+xz0Numt+K0tE8myjv74RjyuY4Q74+X/ZGePYV6O0nhmHxsvgAeDIWs/MFq12yn7YXIz5u/qF746Y56cV5dYWF1ql5FZafA9xcyH93Gg+Y4GT+gJr00at8VUsxGdCRr0ReSNSNqpugme
                m/OP0z3681U61Om7Tkl6sSTZ5vrWnrpPiC+0sSmZLO6kgD4wWCsVzj8K9a+JvhnRoPDGoz2Ok6fDLYXEIgOnJiWJGQFvtIHTuQfdfU15hqfhfxBo0P27VdNnt4jIB5suPmY5Pr7Gug03XPGniLVNa1jTtMS8bUrYWl8qRHyypUKON33to/U+tP21Pl5+ZW73
                0HZmd8OfD9l4m8WxWWohmtYonnkhjOGm2j7gI9SR05wD061H4l8Uafr9vFb2fhmw0doJSVktFAZkxgK/A3H3/AEqfTPCHjvRtRg1HTtJvbe6gbdHIgXIPTp0IIJBB4INbPia28f8Aiq2it73wrFAElMzPa23ltK5GCznccms/rVD+dfeg5X2F+Funadd6P4j
                ur200iaW1FuYJNW+WCMsXB3NjjPH44rh9VuI7zV7q5htobWOSUlYYP9WnsvtWzpXiDXPAU+o6WbC233Pli6tr6HzB8oJXjOOj5/KsTUr+TU9Rn1CdIo5Lhy7JEu1F9gOwroTTV0IrE4Ga9R8L+Bra/wDh3Ilzp8kmrayk1xYXAhJ8kQ42qW/h3nPsQfYVxNt
                4I8T31rFc22i3MsEyh43AGGU9DXY/bPisl7YT2+m3VrDYxJFFZwArAyrwAybueOOfQelYPE0E7Oa+9D5WcH4avrLTfEdhd6laxXVkkoFxFNHvUoeGO3uQDke4Feqal8Lxd6Ytjb6fDBJDrpWO4hUb5bCTB3E/xbN+PbbXBah4J8Y6hqNxeyeHJonuZWlZIkA
                RSxyQBnge1dHFdfFO0kt7lNJn32mnmwQ+TuPlnHzdcl/lXn2+tH1mh/OvvQcr7HIeN7vTrvxdqB0i1gtbCGTyYEgUKrBPl3cddxBOfcV2t34Gt4/hcsI06Rdct7VdWkufJwGjZmBi3+qphivqPevNXgn0y+Ed3aNHNC4ZoLiMjOOcMpwcGup/4Wl4sOty6o9
                4siyqUaxfc1tjbtx5ecds/XPrW6d9UIy/A8EF7430e2uoEuIJblVkikXcrA+o711XxN0mz8P6fb2elaTamyubqWWTVo0Vi0m9v3CkE7FQcY7lenBzw2k6nPoer2+q2ap59tJ5kauMrn3Fb9tc+LfE+kahYWmjteWl/fm9kkS3OI5j12NnA9Mc8VMpxguaTsg
                Rp/CjTtPvn11760064NtZiSFtSA8iNsnlm7D1PpXI+JLiK58Q3ssFrZ2sYk2COxOYMqApKHupI3fjXV+GdK8f+FJLprDw4ZRdx+VMlzCJFZfTGao6x4S8aa3qs2oT+G3hkl2gx28QjjUBQoAXPHArL61Q/nX3ofK+x6NpHgfQ78eF9Si0+ykMWlRvqls0YxI
                skBMcpHTO9WGepz7V4rpEllHrFnLqkby2CzobiNDy0eRuA5HavQbOT4m6dqFrqFtoRR7TTl05U8oFWhXkAjdyQecj+XFee6bfSaVqdvqFuqNLbSLIiyDKkg5wR3FaQq06l+SSfoxNNbnWfE6yhTVbHVNNttNi0a+tz9il06Py0lCsc717SDcAfoOhyB0nwA/
                5DGs/9cI//QjXAeI/FWoeKHtheRW1tb2aFLe0tIvLiiB64XJ64H5V3/wA/wCQxrP/AFwj/wDQjVge4UUUUAeTfH//AJAWk/8AX03/AKDXiX3yqqoB4HXqfWvbfj//AMgLSf8Ar6b/ANBrxHtQAMOoPUV02qO2peDtE1fczS2TPpc7E9l/eRcf7rMP+A1zNdH
                4WP2/Std0FnRWuLb7Xb7hk+bCd2B7lC4rnrqyU+z/AAej/wAy6cuWVzKaYhQwY5U5HtXdRsr+KV8bDa6/2MdQx5fym5A8goPfzOa88TdJhVUuxHCryTXVWurzHwKfDkpMEiXxn3T5CmEDJQd/9YFJA55z61jiKLaXKt9H6Pc66tT2jRzSw3EyCUIxVpBGSeP
                nPQGtSwtLzR9St74Dy7yyuFcITySpyR9OMfjXcaPpemazeWW2xuNLuURRLCpM1u+OjEtzkgk49vXmr+taQmlTm4CR2UsS+Zb3m79zn3zkLyQOoxmsamKXN7NI9Sjhaevtn0v2OebS49H1rxXqNqAYpYlisTtIH+lfNlf92PeKbpOjR6TZy3iXxs75ItsSFC5
                k3cHOOUBGeo/nW7qg17U/BsS+WiXtisklzIzb5SQwC7QvGNrH5ueBx61Rl0xdW0K0W8iX+0dQnWOHyHI8skgb3B6gr2z3z2rGm5xjzVH5P0Wl/na/zJw0KcYThbXv5dvu1uc74ymhsPDuk6TamRUu2k1OZZE2k7zsjOOf4VPfvmt+08Ns/wAPNLtFdVmYnUZ
                oSwG8Odqt+CAHHXk1yuuD/hKfiI1jZYEElyllbbeVWJMRgj2wN1eq6iLDUJ5rGzjiiNuoCTu2EjRVAyCOwA6VFecoRpw6u8n8/wDh/wADny9e0xLqy2X3HG6b4eaUT+SJSkTBnuIuCgXk46/l3qXwu2oS6fNDaORNIZWhujcsrogwNrRpy5Ofw5J4FX9P16y
                0k3WrWN7Je6fo0G2WJQFWaWRtqkHJ5JJOcEYFXNb8FanDod5NYeIY4dImXzrknKfuRyMEZ3HacYJGT9a2pSl7Xkl6/wCf3Jp/M6sXiqdSpJ03pZW8zh/G1vNc6Ro2szOs0yiSwuZ4wdkjRN8rAnk5U4J9VNcc33a7u00+2uvDHiPR0N39oUJqdnFOmAIUXO/
                Ofvsjtkewwa4QnKV30NLw7P8AB6r/AC+R4E/iPWdTsb3WfC3h7S9M/dXM2n25aTkbwB8oJHQD5jWLpOk32keL20HUJzdR36GC9wWLYP3XGf4lOGB9sd67vw7cWa6HpkU8gQjR4S7q5DKpU5PHI47j1rkbdbO18WaJ9meZ54I2edZZDvJZiFJbnoGXgk8cVlg
                G9EvP8ztqQSpcz7Gf4V1bSbET6XqEqNC0haGUZLIehB9V4H45qe5s9I1S98vTrnz3UFy8eR5fpz/hVnXbr4jReItQTTdPvfskdzILdo9NUrsDHbtbZzxjnNT2ravP4ekm8XWsVvei6jXT2uIVhmmPJcbcDIHykHHU9a9vAZzUlUhBxVpNL4rvXysvn5Hl1qU
                W3NaMrJ516i6T4iL6hpp4S5kO6eyJ43q/UgcZU5BArg9Y0u50PWLrSrsDzrWQoSOjDqGHsQQR9a9e0g6oiymCzFzGPv7kBGeuBnqf8a4L4pMr+OrghAkv2eHzVH8L+WvH5YqMxhCnil7NJJ3ul3VtbdHrr30MqFVzcovoN8C+HU1WW51O6tvtUNoQkFqc7bi
                dgSqsf7owS3tjrmugLa++s31lr5d8WReJInAt4FUqRtjHTA+XPX17mr3wdurUaHdLJIqPY3wnYv0CvHsB/NT7c1aTTGuvFcsNtZXl21lHNNfBZzmUSkCMKx9snHI/pwYGUVi3Un9l/clb89zuqQj9Vkzn57TSrCJJbyPzpjCLnbJlkEZOFO0ckZIz6jPT7wy
                tU1P7VYrBHFaWtpGB+74UzjOc7fvL+H9K637JZwx3kVl4cvJtRYbxYaxkJLGAdwi24BcdQDzjOO9cQfEugknd4LtSc85vbj/4qvVrZop1pRtJ29F6dfxOClSTimifwW8A+Ieki23+X9oGNxyeh/SuV27ppOQMEnk11Nh4y0jS7+G/s/B1pFcwNujf7ZO20/Q
                tg1yrYZ2YDAY5A9K82pVnXryqyVlZLW3Rvt6nSkkrBXq3wA/5DGs/9cI//QjXlOOM55z0r1f4Af8AIY1n/rhH/wChGmB7hRRRQM8m+P8A/wAgLSf+vpv/AEGvEa9u+P8A/wAgHSf+vpv/AEGvEsDYG3DOcFe49/8APpQAlaHh7Vm0LxJp+qDdtt5laQL1KdG
                H4qSPxrPpGGRUyipJxezC9jV8RaWdD8R6hpq8LBO3lc9Yzyhz7qQaht5bRDD5y3UpGfOTzAAeT93j09a1vEbR3+j6BrfBaW1Nlc7c5EkJABYnuUZD+FZUF3aW7/6gPkdW5IOOP1rGnKUqSvvs/VaHXS5bpt2/E9P8Dal5j/2NpauLK5UrK1wMvgLk8jsB8o9
                evHSq3iq6Mws4LS4vLS3gMnnRRqHUtnb+JJXkGuat9Tkg8FaxqaboftUkOn22042n/WTED0IVf++qt+KNekttVh1K2AVdXsoroAHIDtw2PQhg/wDSvMVGf1hyXp89G/z/AAPS+tYVtws+Wy+/+rHZaNqX2jTY44rdorkw+ROCxkOF6Eg9PWp7ubStD8Lapqd
                vMzXulgoqlMCKVl8tMA9QGPB+teXf8JTcpfi9inK3WVzOwycKMc+54/w5q94g1HHga1IG2bW7x7iQg8tHENgz9XLH/gNKphJSlFSeja0/F/gmKvi4Ok1Tdv1/pCfDGyf+1NQ1oBcaVZSSIzkhPMZSqhiOgxuP4V0Onwa9OUuI7FVETFp4HmUJKhXcqgkHcSC
                R6dPXFYFjLFoHgvTvNz5upXT3rqo5McXyRq3sW3kdauR+I2uXjjW8lkmdgqouCsi5yBgc7uccGtqilOpKoldbfJf8G5ngrKHLzuN9/wBC/ruh6xq/haJdK0wo+o3jXFwktwqSRIg2xxtvI7l2xSa/a61b+DNNTU7ma1iFuLW8tEcPHIYnBj3FSQCVI9Mkcmu
                d8R+HNZuNUaW18P6oyswZ1+ySkA4HAOOlX9Qs9ag8PS2tr4fvrENJunXypfKTH3mJbIwMD5j0A61UF70ZOa3b89el7/p0RzSio1J67F3Rtcuk1ptdvtYs7TSbgxQywzfNJcRxr5fCKNwO1m54GT3rhNd0x9E12+0t85tp2jBI5ZQeD+Iwfxq14gktrm4img0
                pNKjNvF5cCzGQMuDl8n1Par3jEfb7PRdfUc3tmILgk5Jmh+Rifquw/jXoyi4TjLo9Pnuv1OJu9za1DWV0u/0NJpTDDLoVqomVd3lPhtrEfxLyQw7qTjkCtDw5bS332zXr2ex8128txFNlgo6HBPQ4B47CuX8ark6Cf+oHa/yNP8N+bq72OkWkxtUhlV5Fb5j
                M5YdPQcevetsvg6UYVVG7s3+f9X6E1ZuUXBvQ2LvVrjUrowWcjC1gk8vzXkK5yQD9BnqTwB6nitqx02xPiQaS9wl/ftbeYl3G26G3UAkxLkk9B16k/pFew22v6zq1jpQitJbSadb3TD/y9IpPzxkDOeBlOOSSOTzzGn+IjpxS5hkVJIlCQ5JLxYwCoJOdrDH
                tye4r0cFiViKft1o+q7Pf/hn1OapFx9w7e81//hBIzPqMxeWZC1pYR8mX0dz0Vf1/KvH7/UJ9S1C4v7t/MuLmQySNjHJOfwr0az8Y30cMrx6gWjZFlEDhCFJHzbSQQmDnIxn35pg8ZeILiE21lqkMjHbJ5zW6K6qTjoV5GPTPeuWeExdas6snFt6LVrT05X8
                9Xf02cJU4KyVjjPC3iW48L60l/BGJYmUx3EDdJYz1Hse4PqB9K39fvLnRpIdT8OahcDStTUOkrPubzFGGRwfuspP4549tKXxbrLsskesmKDZtXCRZd88Pyh4OCccdvWue1mfV9dliGqaqZzAg8sSRBEVm5YAL9OTjJ4rGGCxNPEKo4q3Wzbv26L/hjZ1Yyg4
                dGaNj8VfFViQBfC4Uc+XcoJB+Z+YfnVLxW1vr+mQ+L7WzS0lnuGttQhjPyedjcrrznDLnPup65yeee2kQBpGXH1963XUWfw1fziQ+o6mpgQg/MsSHe49suB9aeMoqKjVSs729U91+vyJppK6RzVFA6UVkWBr1j4AsTq2rqcYWBMcDux7968oVijbhjPuM16t
                8AP8AkMaz/wBcI/8A0I0DPcKKKKAPKvjxO9vo+kyoELfaJFxJGrjDRkHhgRnB4PUHkYIrw0DFe3fH/wD5AWk/9fTf+g14jQADGRk4Hc0HGSAcjsaKUjAUbSDjknv9KBHQ6BLZ6h4b1Hw9f6jDYkzR3dnLcZEYkGUcEgE8qR/3zTpfC9nM+9/F+h52heHl7DH
                9z2rmyAetJtFc7oy5m4yav6fqWp6WOg8T3FnBp2k6Fp93DeRWMTyT3EIIWSeRstjPUBQgB+tWbMWXiDwlZ6fdaxaadeaVcSCL7UGCyQyYbqoJJDg8ejVzMKxiaPzy6wlhvKDLbc8lc8E0TCEuwh3lAx2s+ASvbIHQ+vJp+w9xRT1Tvfz1v992K+p0DeFbRwA
                fGOhnHQGSX/4ioPGF9aXusQW2nSiWx061is4JQu0OFHzNj3csaxojHGx3wiUHHBJGOQeMeoBH41Y1O4s7vUZp9Psf7PtHOY7XzTL5Yx/ebk+v40RpNSUpSvb0Bs67xJpematqULWXi3R47K0tYrW2WR5AwRFwcgJjJYseveodFstM8N6j/bdx4i0q/ayjeSC
                2gMjNLNtIj4KjoxBz7Vxm0Uvl4UMRwehrJYV8ns+d29F/kVzsujXNaPJ1e+yeT/pL/wCNaPh3xPe6fr9nPqF/c3FiX8u5illZ1aJhtcFSeflJP4CsGnSRSRFVljaMuodQwxlSMg/Qit5UacouLitSE2dNdeGrGW448Z6M8UY2QmR5d2wfdBATjirN/b6baeA
                7vTW1/Tr2eK7jubSO1LliSNkgJZRxjaf+A1xuwUBQDmjlqOKhKbaunsunyHdb2O61K20jXrPR5k8Sabatb6ZBbyxXBcOroDnop9ak8N2WmaBr1tqI8VaLIkbfOA0udp64+TrXA7QTQyKrFchsHGR0NdEa+IjR9gp+7a2yvZ+YtFLm6mxreqMPGuoavpV0yf6
                bJNbzxnBwWJBH1FadzP4d8VyfbLm8GgaswzOGhL2tw/HzDbzGTySMEfrXKjABG0HIwM9vekIB965lSskotppWuv16MLnTJ4Zto12r4x0MDOfvynB9R8nFNHhe0CgDxlovy/d/eS8Z6/wVzscJlfZGMtgnGewGT+lMCqelPlrf8/H9y/yDTsdOfDVsxZj4z0Y
                lup82Xn/xyk/4Ri227f8AhMtEx6eZL/8AEVzW0elGwUNV3vVf3L/INOx0w0fw7p5E+reJkvwpz9l0yN2eT/to4Cr29T7Vm69rkmvXscn2eO0tLaMQ2lpFysEY6DP8ROcljyT+AGYoQZ3A9Plwe/vRTUHzc05Nvz6fJB6BRRThIViePapDkHcR8y4z0PbrzWg
                hF2k/MSBjqBnntXq3wA/5DGs/9cI//QjXlJBU4YEH0Ner/s/qTqutPkYEMYPIzyx7fhQM9vooooA8m+P/APyAtJ/6+m/9BrxGvbvj/wD8gLSf+vpv/Qa8UbYYo2RSpAxIWcHc2TyBjgYwO/IPPOKAGUZJ6knHHNFOcxkJ5aspC/OWbO5snkcDAxjjnvzQIbR
                RShS2SCBtGTk4z9PWgBCSQASSBwM9qKKUAkEjHHXmgAwSpbBwOCewpKMnGM8elFABRSgZUncBjsep+lJQAUYoooAKKKP8mgAooooAKPpRRQArBSxCE7M8butJRRQArMXIJ7DAA7CkOe1FFAD2WMRlkfJLfKD94Lz1GMenemUUUAOYIEQo5ZyDvUrjac8YPfi
                m0d6KAFJLHJJJPc16r8AP+QxrP/XCP/0I15X5Z8nzdy43bdu4bs+uOuPevVfgB/yGNZ/64R/+hGgZ7hRRRQB5N8f/APkBaT/19N/6DXiNe3fH/wD5AWk/9fTf+g14jQA4eXlSdxH8YHH5H6U2iigQpADEBgwB4Yd6SiigBxkYxLDhdqsWBCjcScZyepHA46D
                n1NNoooAKUDOcsFwM89/akooAKKKKACiiigAoopc/KFwODnPegBKVSFYFlDAdQT1pKXjaABhgTk56igBYTGk8bToZIgwLorbSy55APbjvikcqZGKKVQk7VJyQOwz3oBwclQ3saFO3PAORjn+dAAAvlk7jvzjbjjHrmk5PSiigBW27jsztzxnrilwgbBYkbc5
                Ud8dPz4pFAZgC20E8sRnHvSUAFFKpAJ44PBJGSPpSUAOSR40kRGIWUAOB/EAcj9QKQkkKDjCjA4/GkooAK9X+AH/IY1n/AK4R/wDoRryivV/gB/yGNZ/64R/+hGgD3CiiigZ5N8fuNC0n/r6b/wBBrxE7RkDkdieuK9u+P/8AyAtJ/wCvpv8A0GvElO1lbAb
                HOGGQaABWKNuGM4I5GfakA7Cl3EKU4wTnpz+dAIAIK5J6HPSgQlKoDMAWCgnBY9B70n40rsGIKoqAADC55IHXn160AAALYLADP3u1JUkK27eZ9olkjwhMWxA25+wPIwOvPP0pijccMwXjOT0oACAACGBJ6j+7SUUUAKVKgEjAYZHuKSiigCSSBoreGcyRsJs
                4RXBdcHHzAfdz2z1qOlXaCd2eRxj17UlABRRRQAUUrEFiVXavYZzikoAKKKcUIhWXcp3MV2g/MMY5I9OePoaAG0UDmlViucAHIxyM0AJRSqQrAldw9D3oB+Tbgdc7u/0oASiiigBSCoBIIDDK+4pKKKAFOMnbnHbPWvVvgB/yGNZ/64R/+hGvKK9Z+Abl9W1
                YHbhLaNRhQON7Hn169TQB7dRRRQM8m+P/APyAtJ/6+m/9BrxHkcEYNe3fH/8A5AWk/wDX03/oNeKSStKzs3O45G4lio54BP1oAZRRSlQEDZGT2HpQISigAnoM0uV2EYO7PDZ4A9MflQAlFFFABRRRQAUUUUAFFFFABSnbxtBHHOT3pKKACiilLMwUMxIUYUE
                9B14oACdxycdAOBikxRShWKlgCVXqQOBQAMzOdzHJwB+VJRQTjmgBwZBG6shLnGx92Avrkd802rd3pV5Y6dZahcIq2+oBzbsHUlghw2QDkc+tVQSFK8YNACUUUqgMwBYID/E2cD8qAEooooAK9X+AH/IY1n/rhH/6Ea8or1f4Af8AIY1n/rhH/wChGgD3Cii
                igZ5P8fSF0PSSVDD7U/B/3a8QHSvbvj//AMgLSf8Ar6b/ANBrxGgAooooEKruhJR2UkFSVOMg9R9KSpZLaWK2huX2eXcbvL2yKzfKcHKg5X8QM9qioAVQGYAsEH949BSUUUALkbSu0Zznd3HtSUUUAFGaOtSzT+dFBH5EUfkIV3oCGlyxOW9Tzj6AUARVJJC
                YoYZTJG4mBIVGBZMEjDDseM/So6MUAHbGPxooqTyU+xm4+0R+YJAgt8NvIwTv6bccY6556UATaXYx6nqUNlLe29gkuc3NyxEaYBPJHrjH1NV3QJtxIsm4Z+XPy8kYOR7Z+hFNIyKVjuYttC57DoKAHw+QJUNyJGjDjeseAxT+LBPQ+nBpjlfMby9wjz8oY5O
                O2fekooAKVlKtg4z7HNJ/SigBAMEn1paKKACiineVKYTOI28kMEMmPlDdQM9M4B49qAG0pCgKVJJI+YEYwfb14pKKADrXrPwElebWtYaRyxFtEoJOcAEgD8AAK8mr1f4Af8hjWf8ArhH/AOhGgD3CiiigZ5N8f/8AkBaT/wBfTf8AoNeI17d8f/8AkBaT/wB
                fTf8AoNeI8beTz2HrQAUUUUCDHJPrRRRQAVbGnMdGOq/a7XaLjyPs3mjz87d2/Z/c7Z9aqUY5zQAUUUUAFFFFACkYAOQcjPB6UlFFACgbieQMDPJxSe9FFABxnnp3xRRRQAUUUUAFFFFABRRRQAUZbG3J29dvbNORVbdvkCYXIyCdx9OKFQtG77lGzHyk8tn
                0HegBtPRoRFKskTNIwHlOHwEOecjHORx1FMooAK9X+AH/ACGNZ/64R/8AoRryivV/gB/yGNZ/64R/+hGgD3CiiigZ5N8fz/xI9JH/AE8v/wCg14zbarPa6Ze6dEI/IvvL87dGGb5CSu0nleTzjrX11NbwXAAnhjlCnIDqGx+dR/2fY/8APnb/APfpf8KAPj4
                OAOlS2xt3uolu3kjtywEjxqGZV7kAkAn8RX15/Z9j/wA+dv8A9+l/wo/s6x/587f/AL9L/hQB8f7xk4zjtRvHpX2B/Z1j/wA+dv8A9+l/wo/s+x/587f/AL9L/hQB8f7x6Ubx6V9gf2fY/wDPnb/9+l/wo/s+x/587f8A79L/AIUAfH+8elG8elfYH9n2P/P
                nb/8Afpf8KP7Psf8Anzt/+/S/4UAfH+8elG8elfYH9n2P/Pnb/wDfpf8ACj+z7H/nzt/+/S/4UAfH+8elG8elfYH9n2P/AD52/wD36X/Cj+z7H/nzt/8Av0v+FAHx/vHpRvHpX2B/Z9j/AM+dv/36X/Cj+z7H/nzt/wDv0v8AhQB8f7x6Ubx6V9gf2fY/8+d
                v/wB+l/wo/s+x/wCfO3/79L/hQB8f7x6Ubx6V9gf2fY/8+dv/AN+l/wAKP7Psf+fO3/79L/hQB8f7x6Ubx6V9gf2fY/8APnb/APfpf8KP7Psf+fO3/wC/S/4UAfH+8elODR+UzFmEgYBU28Ec5Oe2OOMc59q+vv7Psf8Anzt/+/S/4Uf2fY/8+dv/AN+l/wA
                KAPj/AHg9Qce1G8elfYH9n2P/AD52/wD36X/Cj+z7H/nzt/8Av0v+FAHx/vHpRvHpX2B/Z9j/AM+dv/36X/Cj+z7H/nzt/wDv0v8AhQB8f7x6V6x8AD/xONY97eP/ANCNe0f2fY/8+dv/AN+l/wAKfFa29uxaG3iiLcEogBP5UATUUUUAf//Z\n\n\n\n\n\n\n\n";
            
            var isBase64 = imageUrl.IsBase64Url();
            
            Assert.True(isBase64);
        }

        [Fact]
        public void IsBase64Url_IsBase64PngImage_ReturnsTrue()
        {
            var imageUrl = @"\ndata:image/png;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAoHBwgHBgoICAgLCgoLDhgQDg0NDh0VFhEYIx8lJCIfIiEmKzcvJik0KSEiMEExNDk7Pj4+JS5ESUM8SDc9Pjv/2wBDAQoLCw4NDhwQEBw7KCIoOzs
                7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozv/wAARCAEsAPIDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaE
                II0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwE
                AAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYa
                HiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwDrvif401PwVp1jc6ZBazNczNG4uUZgABkY2sK87/4Xn4t/6B2kf9+Zf/jldL8fP+QJpH/X23/oNeNYpiZ6B/w
                vPxb/ANA7SP8AvzL/APHKP+F5+Lf+gdpH/fmX/wCOV5/ijFFhXO/b46+LEGTp2kY6f6mX/wCOUg+O/isqWGn6QQOv7mX/AOOVy/hjVbbRdQu57sNtns3gjkVNzROSpDDBBHAPIOea2LnxPoFxq2namLVo/sFyZJLcWq4uFbZk5z/sscNnryckmiw7mifjr4s
                UEnTtI+Xr+5l/+OUn/C9/FeAf7P0fk4H7mX/45WHY65o2l3ulvHbz3S6bazReftWMyO/mkHac8gyIA2eNucHAFW4vFmieZJLcWE0j3tvHa3TGOMyJGEIdt+PncsVYNhSPLUUWC5oj47+Kz00/SD/2xl/+OU7/AIXn4tzj+ztI/wC/Mv8A8crmNX8RPfNatYI
                lk6Rlrt4own2i4bh3I9CAvHABLYAyazJ2099Iht4LSWO/WRjNcNNlJEIG0BccYwe/fv2LBc7c/HjxUDg2Gkdcf6mX/wCOUH48eK1JB0/SMjr+5l/+OVRh8YaXbxRq8Rvbc2tvbNZS2y7bcKqrO6Nn7zAOMjBO/J6UWfivSLVUt4TNbWlrPCUVbVGN3AikOkg
                zj52LMQcj5vagLmj/AMLz8W/9A7SP+/Mv/wAcpD8dfFg/5h+kf9+Zf/jlcjFptu1jJreou1nYzTutpaw8y3BByQmeFRcgFznngAnOG2k895qFvp+lWFrby3MqxRb1EjZJwCXfOPqMUBdnYj45+Lj00zST9IJf/jlH/C8/Fw66dpA+sMv/AMcqDxL4J1jSNQi
                0+w12a9mKAzGWXyUDHoqjcf8ADkVh6jpL+H9QjsPF9nfWrzLuWaC6SQqvTdt5DfTcKNBXOj/4Xn4t/wCgdpH/AH5l/wDjlH/C8/Fv/QO0j/vzL/8AHK5jxX4Zm8IaxHYXE6XkU8IngnQbC6HI5U/dOQeOazp7B0sI9QglEto8nlM2MNDJjOxx2yMkHocH0Io
                C7O3/AOF6eLcE/wBnaRgf9MZf/jlN/wCF8+Kv+fDSP+/Mv/xyuUjvtIh8R6bqFrp8sNhayQPcQSyCUyFcFz0HXB46c9ug6CDxnpMCMl7E2rTs7Mt7PaIHjUshVcEncEKlhk4+bHAoHct/8L48VY/5B+kf9+Zf/jlL/wAL38V8/wDEv0jgZ/1Mv/xys6Lxfp1
                rJYNG1zdzwXUEl5dzRJvuIl3+YnqQQ6jDE9OuAKLLxbpEcNjDf2sty9rcRXkty0SM00wcmQHPJUxsExnH7tTgZoC5pn46eLV66dpHJx/qZf8A45TD8ePFYODp+kcf9MZf/jlcnpdzpuj6nPK+7U7SS0lhCvF5e53QqOMkgAnqDmuhi8YaQiWQiheyhtmja5s
                ktlkS9RYo0MTEnkEo/LZ+/nrRYLl7/hefi7/oHaR/35l/+OUf8Lz8W/8AQO0j/vzL/wDHK89VdoOeOeBnOKdiiwrnoH/C8/Fv/QO0j/vzL/8AHKP+F5+Lf+gdpH/fmX/45Xn+KMUWC56B/wALz8W/9A7SP+/Mv/xyu2+GPxC1jxpqF/b6nbWUK2sSsv2dHUk
                kkc7mPpXhOK9P+Av/ACG9b/64R/zNDQ0z22iiikM8r+O5gGi6T55kC/am/wBWBn7vvXjn2jSv+nz/AL5X/GvXvj//AMgLSf8Ar6b/ANBrxGncTRofaNK/6fP++V/xo8/Sv+n3/vlf8az6KLhY0hPo38X2/wDBU/xqVJ/DX/LUar/wHy6yKe0QWBJfNRi7Mpj
                GdyYxyeMYOeMHsaLisbsc3gr/AJapr3/AWhqws3w8/jTxL+DQVy+KMUXCx1qz/DT+KLxT+DwVItx8Lh9638Un/gcH+Ncdijg0rjsdwt38JgOdO8TMfUtF/wDF1FNcfCpx+7tfFCewaDH6k1xmB6UYHpTuBv6zPo1+9smhS3ggs4fKSHUCvmEFmY7SvB5Y8cH
                61Dp2jnVILmRr+zsUgwP9KfaZGOcBR+HJ9xWKQDTt78fNnHTdzRcVjoYNO1GCX7J/bSIvGBHqGxMHP19M9O4rQsrAaNcyzSW+krfW5BU6ndNMwbGcqi4UnBBG4H9OOVivEjGJLC2mH+1vH8mFWF1aBANmhaaCOjMJW/QuR+lAGhrQ1fVdbSS6uDq+p3fH+ju
                JicEjChRhRwflFXIP7F0LSrjSddmnnuL6aKS4j09kY2yx7sKWPylyWOQM4x1ySBhTa9qk1s9qtwLa2k4eC1RYUcf7QUDd+OazwoFFwOwE/wALxwYPFRH+/BU6XXwoUfNZeJ3+rQ/0YVxOKMD0pDO2kuvhOw+Sx8Txn/ZaH+rGqkk3w2z+6i8T/wDAngrlMD0
                oxTuKx0bz+Bf+WcfiD/gTw/4VA8/hL/lmms/8CMVYfFFFwsazXHh7+Aan+Pl1GZ9F7C//ABCf41m0dKLhY0ftGkdvtv8A3yn+NJ9o0r/p9/75X/Gq09jcW1nbXkqoIbsMYSJFYnacHKg5Xn1Az2qCi47Gh5+lf9Pv/fK/416d8CWtW1rWTbmbmCP/AFgA/iP
                pXkNer/AD/kMaz/1wj/8AQjRcLHuFFFFIZ5N8f/8AkBaT/wBfTf8AoNeI9RXt3x//AOQFpP8A19N/6DXiNACsxdy7HLMcnjFJRRQIKKKlklhe2hjS2WOWPd5kwYky5PGQeBgccde9AEXce9FOWWRI3jViEkxvX+9jpTaAHARiMPvJkDf6vbwV9c5/SnXMwub
                qW4WCK3EjlhDECETPZQSTj8ajqWK0uZ7ae6igeSC22+fIq5WPccLk9snigCKiiigAoopQMqTuAI6A9T9KAEpWAViFYMAeGHQ0lFABQQykqylWHBB4IpyO0UqSxna6EMrDsR0NT6jqN3q+ozajqE3nXc7bpZNoG4/QYA/CgCtRRRQAUqglhjGRz8xwKSigAJy
                ScYyeg7UUpABwGDcZyP5UlABRSZ5x6Uvt3oAO5PrRVqfTb63tPtctjPDbrKbd5XQ7fNHJXOODjt7VVoAK9X+AH/IY1n/rhH/6Ea8or1f4Af8AIY1n/rhH/wChGgD3CiiigZ5N8f8A/kBaT/19N/6DXiNe3fH/AP5AWk/9fTf+g14jQAUUUUCCiiigCS3t57y
                5jtbaJpp5mCRxoMszHoAKjPDFTwQcEelHI6HB9RRQAUmOQfSloJxQAUVLcWs9p5X2hNhmjWWPkHch6HioqACiiigAooooAKKKKACiiigCzp0NpPqUEGo3LWdo7gTTrGXMa+u0cmoJVjWeRYnMkSsQjkYLLng47fSkYlyWcliepJyTSUAFKACG3HGBxxnJpKK
                AJWu7h4IrdpT5UAZY1HGAxyw98n1qKiigB/nzm2FsZpDbq28QljsDYwWx0zjvTKKKAFXbn5gSMHp1z2/WvVfgB/yGNZ/64R/+hGvKa9X+AOP7Y1nH/PCP/wBCNAz3CiiigDyb4/8A/IC0n/r6b/0GvECcDpmvb/j/AP8AIC0n/r6b/wBBrxGgC3qVjDYPbrD
                fwXomt0mYw5xEzdY2yPvDvVSgDFFAhSd2PlAwMcd/ekopQcBhtByMc9vcUAJRRRQAUUVLCLXyp/tBlEuwfZ/LAKl9wzvz0G3d074oAZFI0LFkCkkFfmUMMEYPB789e3UU2j60UAORAyuxkVCi5CtnL8gYGB15zzjgGlggmuriO3t4nlnlYLHGgyzk9AAOppl
                HPUEg9iKADoSO460E+2aKKALeq2UOm6lLaW9/BqESYxcwZ2PkA8Z9M4/CqlAGBiigBWO5s7QvsOlJRSg4UrtHJ6nqP8/0oASiiigAooqVfs32OXeZftW9fKCgeWUw27d3znbjHvntQBFRRRQA4IDEz+YoKkAIc7mznkcYwMc5Pce9Ot7ae7l8m2heaTazbI1
                LHABJOB2ABP4VHRkjoSD7UAGR2r1f4Af8hjWf+uEf/oRryivV/gB/yGNZ/wCuEf8A6EaBnuFFFFAHk3x//wCQFpP/AF9N/wCg14j2x617d8f/APkBaT/19N/6DXiNAC4IUN2PTmkoqU20y2SXpC+Q8jRKd4zuABPy5zjDDnGPyoERUUUjHAoAkhhmuZ0gtoZ
                J5pDtSONSzMfQAcmt0+BtZhCi+m07TZGUMIr2/iikx/ulsj8atvdSeDvDlnHYbotZ1eD7TPdrw8FuSQiIexbG4kYPIFc/pNjHqd6yXNz5QwXLMfvevJ6cZ55rnU51NYu0fvb/AK/EtR6Gr/whd5/0GdB/8GsP+NJ/whd5/wBBnQf/AAaw/wCNZs9hBFPMIJ1
                ngik8sTDoT/nPPfFXrXSoXVUKBpjyB2xSlzxWsvwOzD4GpXdolgeBNTbhdT0Vs+mpxf41PJ8NfEEMMU802mxRTDMTvfRhZB6qc810/hfwvAbZ9V1gGHTLQ/vNq4adu0af1P8ALqM7xZq93q1411KixKFCxwL92GMcKorm9rW/mX3f8E9ZZLTu05vTf16Jd31
                fYwj4E1JeuqaIPrqcX+NMPgq9HXWdBH11SH/Gs64YM5UgKQOQWx/OqUqxkE7ju7ZHH866oRrNXcvwPGr0YU3aLuaup+GL7SdNXUpLmxurVphAZLO6SYK5BYA7TxwDTtM8K6jq2mHU457K1tPNMSyXl0kIdgATjceeoqxHx8Lrj/sNR/8Aol66LX/B2rzaRoW
                kwBEgtLJZGVupnlJZ8/TgfhWDrzXut63avbov6SMqdF1JWirnPHwPqAj8z+1tD2Zxu/tOLH55ofwNqEQUy6voce4bhu1SIZHr16VX0e2vrDXltVtHkf5vNiMSyBlQFmYK3ytt27sH0qHVlu72OTU3Ecgll/fGHlUbt9AefbtW0VVclHn/AAK9kuVt7oq6ppt
                1o2pTadfIEuISNwVgwIIBBBHUEEH8aqE4HNdL4nVr3QdC1k/60Qtp9yMch4T8pPuY2X8q5l+gram5OPvaNXT9VoznZ0s3gPVLWTyrrUdGtpgAWim1KJHXIzyCcjg0weCr0nA1nQSfQarD/jW7470F7zxlI8sv2eG4ijZJGXIY7FBFZen+GLLWNNFtpcrLrNt
                I/mCVv3dwAAQqejf1I55FbrCVvZwqyn7sknsuyv16Amne3Qr/APCEX/8A0F9C/wDBpD/jT0+H+tTHbaXWlXcp5EUGows5+g3U/UPDeox3INvpd80LqHU/Z3zgjPp1qbTopLecqIXingILRuhDr3yR1FewsmpyTcMQnpfZP79TB1mvsnNahpt/pF2bTUrOa0n
                HOyVCpI9R6j3FV69cv9Ps73RoJJnEuh3LiGePq2mTscLPHnpGWwGXgcnA5wPKb2zm07ULmwuQBNbStFIAcjKnB/lXhSU6dR0qm67bP+uqNYyUo8yIm2fLsLHj5tw7+3tSUD3opjCvV/gB/wAhjWf+uEf/AKEa8pG3axJII+6AOv19K9W+AH/IY1n/AK4R/wD
                oRoA9wooooGeTfH//AJAWk/8AX03/AKDXiNe3fH//AJAWk/8AX03/AKDXiNABSsrK5VwVZeGBGCD6Un41dXQ9adFddHv2VhkMLZyCPXpUuUY7uwFMnOOAMDHFNYZWr/8AYOuf9Aa//wDAZ/8ACj+wdb/6A1//AOAz/wCFT7Wn/MvvCzNnxSgv7DQdYhU+Tca
                elqzZB2Sw/Iy47cBSM9Qa5VV/eBTwc45rrfD39o2EE2k6zomozaLdsGlUWz77eQcCaMkcMO/qODWdr3hybR9WurOSdHeFQ6kAjzVI3AgdjtIJB6VhQnGD9lf09P8AgG8Y88fd3R1FprU0vgyGbQtO06WTSYyNRs7m0WWQck/aFY8sp/iH8OPTmsdPiJqqHdH
                pmiqfVbBBUngDTdavPEtvLpm6CdDuMjZChe+7/Zx19c4qt8QNHs9F8WSrpjKbC7QXFvs+6FbIIHtuVse2KwjQo+2cJxve7T/NG1WFWlCM7tX/AKX39DtPAfjvWPE2unSdVFqbBbaSTy44AoUr0IrG8SIoupArZBz+IrI+HsjQ6vqkq/eTSblh+Cisy51GSZ8
                byWPXmoVCKryUFZaHqZXiY0qU3N3udHe6knhzwroclrpOmTzXv2hp5Lu1WZmKuAvJ6DHasr/hOLr/AKF/w9/4LI6m8VMW8IeFieuy6/8ARgrlq1w2Hp1KfNJXd5f+lM8bESftZWOv0nVrvxjq2m+H57LT7Sw+1i5njs7ZYQwRSWJx/sbhXpNx/aXiCxuJhff
                YlztV4kzLgsSASeMYOOACK4H4ZWaoup6rL8u5UsYCVyC0hy2PcKp/76r2i20NE01rdQCJlw+RkAVyVFau1SWkf+Hf6fcdmGcYUXOe7eny3OCubi08JZ114lmSyMdtCX5eZpGHmHI6ERq+PrXI3/hW68Oapcy2t7GiK8vloyb/ADI8jAYH5TuVlOOa7P4heDb
                /AFW0tLHSZ7WCxtWaW4a5n2ZcgBe3QLnH1rC1u8SLT9J0e5u7C4le2W3nmtZRJ5UsZxGzH0K7R+BrXL7SxCV9ZXTXa236/eKtV55yk1pbQyIIBqvhDWrOOOJNoXUYI4ZCUDxHbKQG5HyNnBz93jjpwzHKCu202dfDnijTobqNFtoJGiuAwxvR8o5b1+U5rld
                a02TR9ZvdMlJLWs7xZIxuAOAfxHNe7iqE6Fe0/tJS/R/kvvPLUlJXR6z4oS4vNXGj3jFre4jjks0I27m2qNob0OD+Ix3qho+q2UfjHTp7029hcKRBMsMXlREAkKWbPzN0ySB0rq7iKy8bfa/Dc6fZ9R0tUa1uQOGBRSQT65P9fWuB17w9eWiSIbOae4ZsPuB
                EhbvjBOcivYoUaeKy2NGXuzUF+K3+a/E5qVR060tdGyqdJ+IhdwNYmzkkj+2o/wD45xV661690PSdPt9W1RLrWEeTzWSQT7LfgrDKwOHy4JAycD64rkLJYP3oVgC6HB7k9fy4xUlrLEj7TGu1+Gz0x7+1ceDyPmSquaTi3oo26d7v7jeVble1zoLPUrnVjcW
                Gi2k063n7lY3+ZUDcHeQPrzxjHtWD42uIrrxzrMsGPLN24BB4ODgn8SCa7vUrnUfB+nG10rTp73U7qEFL21t2a2t0I/5ZNj52x34Az35z5suga2Bxo1+ff7M/+FebiMXRr1U4NcsU1fq299+isrd9S0nq2rXKVBOTk96vf2FrYU50a/C9T/oz/wCFUaiMoy+
                F3CwV6v8AAD/kMaz/ANcI/wD0I15ZbpA7sLmZoVCMVZY9+5gPlXGRgE8Z7ehr1T4Bbf7Z1jbnH2ePr65OaoD2+iiigDyb4/8A/IC0n/r6b/0GvEa9u+P/APyAtJ/6+m/9BrxAkDGaAEf7oOQc/pXd+OvEGrWvja8tIdZ1C0gVIAiQXDKiZhQ8LuAHJJrhXJl
                Jfuxye1dr4hl8H+I9al1aTxDd2jTpGDB/ZxfYVRV+9vGfu1y1lH2sZSjdWfS/VFRdloYreJtdBITxLfPwSD9rmB/njNNXxTrxbe3iDUgc/c+2SYP61bGleDR08V3n/grP/wAXTjp3g8rt/wCEruwPT+yj/wDF0c1D+T/yV/5Gil3LujeNNcsLtJ21a7mMZyR
                NM0in2IY4IrT8QW2leIby2u9Lilt7u4IM9rKx4dv4kYn5lPfPT6dMGKx8GxOGHim9IHQf2Yf/AIupBF4X+0NcN4y1BpGYuz/2acsT1531z2pxbcE1/wBuv/I9elmFNRipxv8A117+XVHU6/qdr4O0dvDmkSLNdzqFvrmI7vNJ6RJ/sjOD69PUVyPj59mrWGm
                uwafTdPiguCCCBKdzsMj0L4/Cr934t0DTGjl8O6bJcajGmFvr1QEjb++kWSN3cZOAe1cU8rSyvJLI0kkjFndjksTySTWlCnKU1OSslt/X9XZy4/Fqr7kHdb+r7/ol0Wh0fgg4utcP/UFu/wD0CsC0y8oHcmtjwjqNhpuoXo1OWSG3vbCa182OPeULjAOM81b
                t7Dwbbvu/4Sm8Yj/qFkf+z1UpclSbaetujf5HJQlFSjzPQk8Wrs8J+F19Euf/AEYK5NuFrp/GGq6Rfafo9jpNzNcrYLKJJZYvLyXYEYGT71zUPlSXESzMyw7x5jKMkLnkj8KvCJxormVnd/i2xYmUZVpOO1z1zwfpUlpoWjx/ZWkVEbUJ8H77yf6tQO7bFHH
                +1XpFprcRs4mkYebOQEiGNxJ6KPw5ryu+8Y+HbiZ5LTxDdWWHQ24i04kQqoAA+/ycKoz6DpVfUPGegJa3t9bahdXurS2rQQhrbykR3+VpByQDgn/61eUlW+KKfM/J9e/kjulPDujGN9l+J6JrElzqFtdosdu6EhIl89MuO7dRjsK8j17wh4hijuL7+zCtrAp
                eVknjbCjqcBs4ri9nqa1vDGrRaH4htru5QyWjbormMc7onBV+O/BJ+oFenRjicMnKMlJ9rf8ABOSWJcqfs7WRLf38uqW0c9wd8yfKznqR2q543Bvl0jX+SdSslSZieWni/dufxAU/jUq2PgtFZf8AhKLxgTx/xLD/APF0urXXh1fBx0mw1Se/uUvRcws9mYt
                gK7XXO48HCn8K9rM8bh8VGlKnfmWj91rR77q29mcVODjddDsrrxGdM8Z3cQm8sxGIg/8AbNa7K2uNP8aTWeqWcqpe6dMrXEWfvL6j1/yK8o1i68H63rMurPr93bvOELQHTi+0hQpGd4z0q94X13wt4X1RLyDxHezIuQ0X9nFd6nqCd59u3aoeLw9TLlB3VVQ
                5V7stfdtZ6W1InS99SXcy5IYPGFtLqulxJBrUAMl7Yx9Jx3liHr/eX8R785HKVbIxz61Wtry4sL5L6zle3niffG6HBU100mseGPEDm51aG60nUX5lms4xLBK3dzGSCpPsSO/etMBmDwsfZVU3Do92vJ9WuzWq2LnDm1RJofiO/wBPX7Pa6hcwRE5EaSsFz9M
                1n3XizxBPeTSprmpIruSqi7kAA7d6uC28GKCB4nvenUaYf/i6QWXgr/oZrz/wWH/4uvSxWYZZWaajru/cf+RnGE1/w5d8EeIdcufGmlwXGs380Uk2HjkunZWGD1BODXEH/Wv/ALxrudCufBeh61baoNfvbhrV94i/s4rv4Ixnecda4Y5LMxGMnNeBXdCWJc6
                C93lXRx1u+6Rsr8uorMzsWdizHqSck16t8AP+QxrP/XCP/wBCNeUA5GRXq/wA/wCQxrP/AFwj/wDQjUDPcKKKKAPJvj//AMgLSf8Ar6b/ANBriPhL/Z154p/sbUtHs9RjvlJElwgZodiO3y59TjP0Fdv8f/8AkBaT/wBfTf8AoNeReH9dvfDOsRatp4jNzEr
                KvmruX5lKnjI7GgCvrF/HqWq3F7BZQWEcrZFtbjCR8AcD8M/jXsPiLwhotpD4mv4rbSdq6QjQWcQHnWr45kK4+XPr3rxRgCSW4yc16JJN8RNXN/qC6CrrrNklvI6RYBixwV+bg4PWonUhT1nJL1BK5ifC6wtNU+IOnWd/bR3VvIJd8UqhlOImIyD7gVD46nt
                pdfEFtaaNCtvGFMmjPuglz8wOfUZwfpV/w/4Z8d+Gtag1bT9Bm+0wBgnmqCvzKVORkdiayfEOqap4j1qK1utNtra9hf7MttZ2/l5cucgju244pQrU535ZJ27MLMw8D0r0P4Xadp91ofiK8vLTR5prU23kyawdsEe5mBy2OM8fjiuc/wCEB8XH/mAXf/fI/wA
                a0rLw54607QNT0iLw/KbXUvLNwWQFv3Z3Lt59fY1H1qh/z8X3ofK+xz0Numt+K0tE8myjv74RjyuY4Q74+X/ZGePYV6O0nhmHxsvgAeDIWs/MFq12yn7YXIz5u/qF746Y56cV5dYWF1ql5FZafA9xcyH93Gg+Y4GT+gJr00at8VUsxGdCRr0ReSNSNqpugme
                m/OP0z3681U61Om7Tkl6sSTZ5vrWnrpPiC+0sSmZLO6kgD4wWCsVzj8K9a+JvhnRoPDGoz2Ok6fDLYXEIgOnJiWJGQFvtIHTuQfdfU15hqfhfxBo0P27VdNnt4jIB5suPmY5Pr7Gug03XPGniLVNa1jTtMS8bUrYWl8qRHyypUKON33to/U+tP21Pl5+ZW73
                0HZmd8OfD9l4m8WxWWohmtYonnkhjOGm2j7gI9SR05wD061H4l8Uafr9vFb2fhmw0doJSVktFAZkxgK/A3H3/AEqfTPCHjvRtRg1HTtJvbe6gbdHIgXIPTp0IIJBB4INbPia28f8Aiq2it73wrFAElMzPa23ltK5GCznccms/rVD+dfeg5X2F+Funadd6P4j
                ur200iaW1FuYJNW+WCMsXB3NjjPH44rh9VuI7zV7q5htobWOSUlYYP9WnsvtWzpXiDXPAU+o6WbC233Pli6tr6HzB8oJXjOOj5/KsTUr+TU9Rn1CdIo5Lhy7JEu1F9gOwroTTV0IrE4Ga9R8L+Bra/wDh3Ilzp8kmrayk1xYXAhJ8kQ42qW/h3nPsQfYVxNt
                4I8T31rFc22i3MsEyh43AGGU9DXY/bPisl7YT2+m3VrDYxJFFZwArAyrwAybueOOfQelYPE0E7Oa+9D5WcH4avrLTfEdhd6laxXVkkoFxFNHvUoeGO3uQDke4Feqal8Lxd6Ytjb6fDBJDrpWO4hUb5bCTB3E/xbN+PbbXBah4J8Y6hqNxeyeHJonuZWlZIkA
                RSxyQBnge1dHFdfFO0kt7lNJn32mnmwQ+TuPlnHzdcl/lXn2+tH1mh/OvvQcr7HIeN7vTrvxdqB0i1gtbCGTyYEgUKrBPl3cddxBOfcV2t34Gt4/hcsI06Rdct7VdWkufJwGjZmBi3+qphivqPevNXgn0y+Ed3aNHNC4ZoLiMjOOcMpwcGup/4Wl4sOty6o9
                4siyqUaxfc1tjbtx5ecds/XPrW6d9UIy/A8EF7430e2uoEuIJblVkikXcrA+o711XxN0mz8P6fb2elaTamyubqWWTVo0Vi0m9v3CkE7FQcY7lenBzw2k6nPoer2+q2ap59tJ5kauMrn3Fb9tc+LfE+kahYWmjteWl/fm9kkS3OI5j12NnA9Mc8VMpxguaTsg
                Rp/CjTtPvn11760064NtZiSFtSA8iNsnlm7D1PpXI+JLiK58Q3ssFrZ2sYk2COxOYMqApKHupI3fjXV+GdK8f+FJLprDw4ZRdx+VMlzCJFZfTGao6x4S8aa3qs2oT+G3hkl2gx28QjjUBQoAXPHArL61Q/nX3ofK+x6NpHgfQ78eF9Si0+ykMWlRvqls0YxI
                skBMcpHTO9WGepz7V4rpEllHrFnLqkby2CzobiNDy0eRuA5HavQbOT4m6dqFrqFtoRR7TTl05U8oFWhXkAjdyQecj+XFee6bfSaVqdvqFuqNLbSLIiyDKkg5wR3FaQq06l+SSfoxNNbnWfE6yhTVbHVNNttNi0a+tz9il06Py0lCsc717SDcAfoOhyB0nwA/
                5DGs/9cI//QjXAeI/FWoeKHtheRW1tb2aFLe0tIvLiiB64XJ64H5V3/wA/wCQxrP/AFwj/wDQjVge4UUUUAeTfH//AJAWk/8AX03/AKDXiX3yqqoB4HXqfWvbfj//AMgLSf8Ar6b/ANBrxHtQAMOoPUV02qO2peDtE1fczS2TPpc7E9l/eRcf7rMP+A1zNdH
                4WP2/Std0FnRWuLb7Xb7hk+bCd2B7lC4rnrqyU+z/AAej/wAy6cuWVzKaYhQwY5U5HtXdRsr+KV8bDa6/2MdQx5fym5A8goPfzOa88TdJhVUuxHCryTXVWurzHwKfDkpMEiXxn3T5CmEDJQd/9YFJA55z61jiKLaXKt9H6Pc66tT2jRzSw3EyCUIxVpBGSeP
                nPQGtSwtLzR9St74Dy7yyuFcITySpyR9OMfjXcaPpemazeWW2xuNLuURRLCpM1u+OjEtzkgk49vXmr+taQmlTm4CR2UsS+Zb3m79zn3zkLyQOoxmsamKXN7NI9Sjhaevtn0v2OebS49H1rxXqNqAYpYlisTtIH+lfNlf92PeKbpOjR6TZy3iXxs75ItsSFC5
                k3cHOOUBGeo/nW7qg17U/BsS+WiXtisklzIzb5SQwC7QvGNrH5ueBx61Rl0xdW0K0W8iX+0dQnWOHyHI8skgb3B6gr2z3z2rGm5xjzVH5P0Wl/na/zJw0KcYThbXv5dvu1uc74ymhsPDuk6TamRUu2k1OZZE2k7zsjOOf4VPfvmt+08Ns/wAPNLtFdVmYnUZ
                oSwG8Odqt+CAHHXk1yuuD/hKfiI1jZYEElyllbbeVWJMRgj2wN1eq6iLDUJ5rGzjiiNuoCTu2EjRVAyCOwA6VFecoRpw6u8n8/wDh/wADny9e0xLqy2X3HG6b4eaUT+SJSkTBnuIuCgXk46/l3qXwu2oS6fNDaORNIZWhujcsrogwNrRpy5Ofw5J4FX9P16y
                0k3WrWN7Je6fo0G2WJQFWaWRtqkHJ5JJOcEYFXNb8FanDod5NYeIY4dImXzrknKfuRyMEZ3HacYJGT9a2pSl7Xkl6/wCf3Jp/M6sXiqdSpJ03pZW8zh/G1vNc6Ro2szOs0yiSwuZ4wdkjRN8rAnk5U4J9VNcc33a7u00+2uvDHiPR0N39oUJqdnFOmAIUXO/
                Ofvsjtkewwa4QnKV30NLw7P8AB6r/AC+R4E/iPWdTsb3WfC3h7S9M/dXM2n25aTkbwB8oJHQD5jWLpOk32keL20HUJzdR36GC9wWLYP3XGf4lOGB9sd67vw7cWa6HpkU8gQjR4S7q5DKpU5PHI47j1rkbdbO18WaJ9meZ54I2edZZDvJZiFJbnoGXgk8cVlg
                G9EvP8ztqQSpcz7Gf4V1bSbET6XqEqNC0haGUZLIehB9V4H45qe5s9I1S98vTrnz3UFy8eR5fpz/hVnXbr4jReItQTTdPvfskdzILdo9NUrsDHbtbZzxjnNT2ravP4ekm8XWsVvei6jXT2uIVhmmPJcbcDIHykHHU9a9vAZzUlUhBxVpNL4rvXysvn5Hl1qU
                W3NaMrJ516i6T4iL6hpp4S5kO6eyJ43q/UgcZU5BArg9Y0u50PWLrSrsDzrWQoSOjDqGHsQQR9a9e0g6oiymCzFzGPv7kBGeuBnqf8a4L4pMr+OrghAkv2eHzVH8L+WvH5YqMxhCnil7NJJ3ul3VtbdHrr30MqFVzcovoN8C+HU1WW51O6tvtUNoQkFqc7bi
                dgSqsf7owS3tjrmugLa++s31lr5d8WReJInAt4FUqRtjHTA+XPX17mr3wdurUaHdLJIqPY3wnYv0CvHsB/NT7c1aTTGuvFcsNtZXl21lHNNfBZzmUSkCMKx9snHI/pwYGUVi3Un9l/clb89zuqQj9Vkzn57TSrCJJbyPzpjCLnbJlkEZOFO0ckZIz6jPT7wy
                tU1P7VYrBHFaWtpGB+74UzjOc7fvL+H9K637JZwx3kVl4cvJtRYbxYaxkJLGAdwi24BcdQDzjOO9cQfEugknd4LtSc85vbj/4qvVrZop1pRtJ29F6dfxOClSTimifwW8A+Ieki23+X9oGNxyeh/SuV27ppOQMEnk11Nh4y0jS7+G/s/B1pFcwNujf7ZO20/Q
                tg1yrYZ2YDAY5A9K82pVnXryqyVlZLW3Rvt6nSkkrBXq3wA/5DGs/9cI//QjXlOOM55z0r1f4Af8AIY1n/rhH/wChGmB7hRRRQM8m+P8A/wAgLSf+vpv/AEGvEa9u+P8A/wAgHSf+vpv/AEGvEsDYG3DOcFe49/8APpQAlaHh7Vm0LxJp+qDdtt5laQL1KdG
                H4qSPxrPpGGRUyipJxezC9jV8RaWdD8R6hpq8LBO3lc9Yzyhz7qQaht5bRDD5y3UpGfOTzAAeT93j09a1vEbR3+j6BrfBaW1Nlc7c5EkJABYnuUZD+FZUF3aW7/6gPkdW5IOOP1rGnKUqSvvs/VaHXS5bpt2/E9P8Dal5j/2NpauLK5UrK1wMvgLk8jsB8o9
                evHSq3iq6Mws4LS4vLS3gMnnRRqHUtnb+JJXkGuat9Tkg8FaxqaboftUkOn22042n/WTED0IVf++qt+KNekttVh1K2AVdXsoroAHIDtw2PQhg/wDSvMVGf1hyXp89G/z/AAPS+tYVtws+Wy+/+rHZaNqX2jTY44rdorkw+ROCxkOF6Eg9PWp7ubStD8Lapqd
                vMzXulgoqlMCKVl8tMA9QGPB+teXf8JTcpfi9inK3WVzOwycKMc+54/w5q94g1HHga1IG2bW7x7iQg8tHENgz9XLH/gNKphJSlFSeja0/F/gmKvi4Ok1Tdv1/pCfDGyf+1NQ1oBcaVZSSIzkhPMZSqhiOgxuP4V0Onwa9OUuI7FVETFp4HmUJKhXcqgkHcSC
                R6dPXFYFjLFoHgvTvNz5upXT3rqo5McXyRq3sW3kdauR+I2uXjjW8lkmdgqouCsi5yBgc7uccGtqilOpKoldbfJf8G5ngrKHLzuN9/wBC/ruh6xq/haJdK0wo+o3jXFwktwqSRIg2xxtvI7l2xSa/a61b+DNNTU7ma1iFuLW8tEcPHIYnBj3FSQCVI9Mkcmu
                d8R+HNZuNUaW18P6oyswZ1+ySkA4HAOOlX9Qs9ag8PS2tr4fvrENJunXypfKTH3mJbIwMD5j0A61UF70ZOa3b89el7/p0RzSio1J67F3Rtcuk1ptdvtYs7TSbgxQywzfNJcRxr5fCKNwO1m54GT3rhNd0x9E12+0t85tp2jBI5ZQeD+Iwfxq14gktrm4img0
                pNKjNvF5cCzGQMuDl8n1Par3jEfb7PRdfUc3tmILgk5Jmh+Rifquw/jXoyi4TjLo9Pnuv1OJu9za1DWV0u/0NJpTDDLoVqomVd3lPhtrEfxLyQw7qTjkCtDw5bS332zXr2ex8128txFNlgo6HBPQ4B47CuX8ark6Cf+oHa/yNP8N+bq72OkWkxtUhlV5Fb5j
                M5YdPQcevetsvg6UYVVG7s3+f9X6E1ZuUXBvQ2LvVrjUrowWcjC1gk8vzXkK5yQD9BnqTwB6nitqx02xPiQaS9wl/ftbeYl3G26G3UAkxLkk9B16k/pFew22v6zq1jpQitJbSadb3TD/y9IpPzxkDOeBlOOSSOTzzGn+IjpxS5hkVJIlCQ5JLxYwCoJOdrDH
                tye4r0cFiViKft1o+q7Pf/hn1OapFx9w7e81//hBIzPqMxeWZC1pYR8mX0dz0Vf1/KvH7/UJ9S1C4v7t/MuLmQySNjHJOfwr0az8Y30cMrx6gWjZFlEDhCFJHzbSQQmDnIxn35pg8ZeILiE21lqkMjHbJ5zW6K6qTjoV5GPTPeuWeExdas6snFt6LVrT05X8
                9Xf02cJU4KyVjjPC3iW48L60l/BGJYmUx3EDdJYz1Hse4PqB9K39fvLnRpIdT8OahcDStTUOkrPubzFGGRwfuspP4549tKXxbrLsskesmKDZtXCRZd88Pyh4OCccdvWue1mfV9dliGqaqZzAg8sSRBEVm5YAL9OTjJ4rGGCxNPEKo4q3Wzbv26L/hjZ1Yyg4
                dGaNj8VfFViQBfC4Uc+XcoJB+Z+YfnVLxW1vr+mQ+L7WzS0lnuGttQhjPyedjcrrznDLnPup65yeee2kQBpGXH1963XUWfw1fziQ+o6mpgQg/MsSHe49suB9aeMoqKjVSs729U91+vyJppK6RzVFA6UVkWBr1j4AsTq2rqcYWBMcDux7968oVijbhjPuM16t
                8AP8AkMaz/wBcI/8A0I0DPcKKKKAPKvjxO9vo+kyoELfaJFxJGrjDRkHhgRnB4PUHkYIrw0DFe3fH/wD5AWk/9fTf+g14jQADGRk4Hc0HGSAcjsaKUjAUbSDjknv9KBHQ6BLZ6h4b1Hw9f6jDYkzR3dnLcZEYkGUcEgE8qR/3zTpfC9nM+9/F+h52heHl7DH
                9z2rmyAetJtFc7oy5m4yav6fqWp6WOg8T3FnBp2k6Fp93DeRWMTyT3EIIWSeRstjPUBQgB+tWbMWXiDwlZ6fdaxaadeaVcSCL7UGCyQyYbqoJJDg8ejVzMKxiaPzy6wlhvKDLbc8lc8E0TCEuwh3lAx2s+ASvbIHQ+vJp+w9xRT1Tvfz1v992K+p0DeFbRwA
                fGOhnHQGSX/4ioPGF9aXusQW2nSiWx061is4JQu0OFHzNj3csaxojHGx3wiUHHBJGOQeMeoBH41Y1O4s7vUZp9Psf7PtHOY7XzTL5Yx/ebk+v40RpNSUpSvb0Bs67xJpematqULWXi3R47K0tYrW2WR5AwRFwcgJjJYseveodFstM8N6j/bdx4i0q/ayjeSC
                2gMjNLNtIj4KjoxBz7Vxm0Uvl4UMRwehrJYV8ns+d29F/kVzsujXNaPJ1e+yeT/pL/wCNaPh3xPe6fr9nPqF/c3FiX8u5illZ1aJhtcFSeflJP4CsGnSRSRFVljaMuodQwxlSMg/Qit5UacouLitSE2dNdeGrGW448Z6M8UY2QmR5d2wfdBATjirN/b6baeA
                7vTW1/Tr2eK7jubSO1LliSNkgJZRxjaf+A1xuwUBQDmjlqOKhKbaunsunyHdb2O61K20jXrPR5k8Sabatb6ZBbyxXBcOroDnop9ak8N2WmaBr1tqI8VaLIkbfOA0udp64+TrXA7QTQyKrFchsHGR0NdEa+IjR9gp+7a2yvZ+YtFLm6mxreqMPGuoavpV0yf6
                bJNbzxnBwWJBH1FadzP4d8VyfbLm8GgaswzOGhL2tw/HzDbzGTySMEfrXKjABG0HIwM9vekIB965lSskotppWuv16MLnTJ4Zto12r4x0MDOfvynB9R8nFNHhe0CgDxlovy/d/eS8Z6/wVzscJlfZGMtgnGewGT+lMCqelPlrf8/H9y/yDTsdOfDVsxZj4z0Y
                lup82Xn/xyk/4Ri227f8AhMtEx6eZL/8AEVzW0elGwUNV3vVf3L/INOx0w0fw7p5E+reJkvwpz9l0yN2eT/to4Cr29T7Vm69rkmvXscn2eO0tLaMQ2lpFysEY6DP8ROcljyT+AGYoQZ3A9Plwe/vRTUHzc05Nvz6fJB6BRRThIViePapDkHcR8y4z0PbrzWg
                hF2k/MSBjqBnntXq3wA/5DGs/9cI//QjXlJBU4YEH0Ner/s/qTqutPkYEMYPIzyx7fhQM9vooooA8m+P/APyAtJ/6+m/9BrxGvbvj/wD8gLSf+vpv/Qa8UbYYo2RSpAxIWcHc2TyBjgYwO/IPPOKAGUZJ6knHHNFOcxkJ5aspC/OWbO5snkcDAxjjnvzQIbR
                RShS2SCBtGTk4z9PWgBCSQASSBwM9qKKUAkEjHHXmgAwSpbBwOCewpKMnGM8elFABRSgZUncBjsep+lJQAUYoooAKKKP8mgAooooAKPpRRQArBSxCE7M8butJRRQArMXIJ7DAA7CkOe1FFAD2WMRlkfJLfKD94Lz1GMenemUUUAOYIEQo5ZyDvUrjac8YPfi
                m0d6KAFJLHJJJPc16r8AP+QxrP/XCP/0I15X5Z8nzdy43bdu4bs+uOuPevVfgB/yGNZ/64R/+hGgZ7hRRRQB5N8f/APkBaT/19N/6DXiNe3fH/wD5AWk/9fTf+g14jQA4eXlSdxH8YHH5H6U2iigQpADEBgwB4Yd6SiigBxkYxLDhdqsWBCjcScZyepHA46D
                n1NNoooAKUDOcsFwM89/akooAKKKKACiiigAoopc/KFwODnPegBKVSFYFlDAdQT1pKXjaABhgTk56igBYTGk8bToZIgwLorbSy55APbjvikcqZGKKVQk7VJyQOwz3oBwclQ3saFO3PAORjn+dAAAvlk7jvzjbjjHrmk5PSiigBW27jsztzxnrilwgbBYkbc5
                Ud8dPz4pFAZgC20E8sRnHvSUAFFKpAJ44PBJGSPpSUAOSR40kRGIWUAOB/EAcj9QKQkkKDjCjA4/GkooAK9X+AH/IY1n/AK4R/wDoRryivV/gB/yGNZ/64R/+hGgD3CiiigZ5N8fuNC0n/r6b/wBBrxE7RkDkdieuK9u+P/8AyAtJ/wCvpv8A0GvElO1lbAb
                HOGGQaABWKNuGM4I5GfakA7Cl3EKU4wTnpz+dAIAIK5J6HPSgQlKoDMAWCgnBY9B70n40rsGIKoqAADC55IHXn160AAALYLADP3u1JUkK27eZ9olkjwhMWxA25+wPIwOvPP0pijccMwXjOT0oACAACGBJ6j+7SUUUAKVKgEjAYZHuKSiigCSSBoreGcyRsJs
                4RXBdcHHzAfdz2z1qOlXaCd2eRxj17UlABRRRQAUUrEFiVXavYZzikoAKKKcUIhWXcp3MV2g/MMY5I9OePoaAG0UDmlViucAHIxyM0AJRSqQrAldw9D3oB+Tbgdc7u/0oASiiigBSCoBIIDDK+4pKKKAFOMnbnHbPWvVvgB/yGNZ/64R/+hGvKK9Z+Abl9W1
                YHbhLaNRhQON7Hn169TQB7dRRRQM8m+P/APyAtJ/6+m/9BrxHkcEYNe3fH/8A5AWk/wDX03/oNeKSStKzs3O45G4lio54BP1oAZRRSlQEDZGT2HpQISigAnoM0uV2EYO7PDZ4A9MflQAlFFFABRRRQAUUUUAFFFFABSnbxtBHHOT3pKKACiilLMwUMxIUYUE
                9B14oACdxycdAOBikxRShWKlgCVXqQOBQAMzOdzHJwB+VJRQTjmgBwZBG6shLnGx92Avrkd802rd3pV5Y6dZahcIq2+oBzbsHUlghw2QDkc+tVQSFK8YNACUUUqgMwBYID/E2cD8qAEooooAK9X+AH/IY1n/rhH/6Ea8or1f4Af8AIY1n/rhH/wChGgD3Cii
                igZ5P8fSF0PSSVDD7U/B/3a8QHSvbvj//AMgLSf8Ar6b/ANBrxGgAooooEKruhJR2UkFSVOMg9R9KSpZLaWK2huX2eXcbvL2yKzfKcHKg5X8QM9qioAVQGYAsEH949BSUUUALkbSu0Zznd3HtSUUUAFGaOtSzT+dFBH5EUfkIV3oCGlyxOW9Tzj6AUARVJJC
                YoYZTJG4mBIVGBZMEjDDseM/So6MUAHbGPxooqTyU+xm4+0R+YJAgt8NvIwTv6bccY6556UATaXYx6nqUNlLe29gkuc3NyxEaYBPJHrjH1NV3QJtxIsm4Z+XPy8kYOR7Z+hFNIyKVjuYttC57DoKAHw+QJUNyJGjDjeseAxT+LBPQ+nBpjlfMby9wjz8oY5O
                O2fekooAKVlKtg4z7HNJ/SigBAMEn1paKKACiineVKYTOI28kMEMmPlDdQM9M4B49qAG0pCgKVJJI+YEYwfb14pKKADrXrPwElebWtYaRyxFtEoJOcAEgD8AAK8mr1f4Af8hjWf8ArhH/AOhGgD3CiiigZ5N8f/8AkBaT/wBfTf8AoNeI17d8f/8AkBaT/wB
                fTf8AoNeI8beTz2HrQAUUUUCDHJPrRRRQAVbGnMdGOq/a7XaLjyPs3mjz87d2/Z/c7Z9aqUY5zQAUUUUAFFFFACkYAOQcjPB6UlFFACgbieQMDPJxSe9FFABxnnp3xRRRQAUUUUAFFFFABRRRQAUZbG3J29dvbNORVbdvkCYXIyCdx9OKFQtG77lGzHyk8tn
                0HegBtPRoRFKskTNIwHlOHwEOecjHORx1FMooAK9X+AH/ACGNZ/64R/8AoRryivV/gB/yGNZ/64R/+hGgD3CiiigZ5N8fz/xI9JH/AE8v/wCg14zbarPa6Ze6dEI/IvvL87dGGb5CSu0nleTzjrX11NbwXAAnhjlCnIDqGx+dR/2fY/8APnb/APfpf8KAPj4
                OAOlS2xt3uolu3kjtywEjxqGZV7kAkAn8RX15/Z9j/wA+dv8A9+l/wo/s6x/587f/AL9L/hQB8f7xk4zjtRvHpX2B/Z1j/wA+dv8A9+l/wo/s+x/587f/AL9L/hQB8f7x6Ubx6V9gf2fY/wDPnb/9+l/wo/s+x/587f8A79L/AIUAfH+8elG8elfYH9n2P/P
                nb/8Afpf8KP7Psf8Anzt/+/S/4UAfH+8elG8elfYH9n2P/Pnb/wDfpf8ACj+z7H/nzt/+/S/4UAfH+8elG8elfYH9n2P/AD52/wD36X/Cj+z7H/nzt/8Av0v+FAHx/vHpRvHpX2B/Z9j/AM+dv/36X/Cj+z7H/nzt/wDv0v8AhQB8f7x6Ubx6V9gf2fY/8+d
                v/wB+l/wo/s+x/wCfO3/79L/hQB8f7x6Ubx6V9gf2fY/8+dv/AN+l/wAKP7Psf+fO3/79L/hQB8f7x6Ubx6V9gf2fY/8APnb/APfpf8KP7Psf+fO3/wC/S/4UAfH+8elODR+UzFmEgYBU28Ec5Oe2OOMc59q+vv7Psf8Anzt/+/S/4Uf2fY/8+dv/AN+l/wA
                KAPj/AHg9Qce1G8elfYH9n2P/AD52/wD36X/Cj+z7H/nzt/8Av0v+FAHx/vHpRvHpX2B/Z9j/AM+dv/36X/Cj+z7H/nzt/wDv0v8AhQB8f7x6V6x8AD/xONY97eP/ANCNe0f2fY/8+dv/AN+l/wAKfFa29uxaG3iiLcEogBP5UATUUUUAf//Z\n\n\n\n\n\n\n\n";
            
            var isBase64 = imageUrl.IsBase64Url();
            
            Assert.True(isBase64);
        }

        [Fact]
        public void IsBase64Url_IsNotBase64_ReturnsFalse()
        {
            var imageUrl = "not base64";

            var isBase64 = imageUrl.IsBase64Url();

            Assert.False(isBase64);
        }
    }
}
